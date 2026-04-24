using ConnectDB.Models;
using ConnectDB.Services;
using Microsoft.AspNetCore.Mvc;

namespace ConnectDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VnPayController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public VnPayController(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("create-payment")]
        public IActionResult CreatePaymentUrl(int orderId, decimal amount)
        {
            var vnpay = new VnPayLibrary();
            var url = _configuration["VnPay:Url"];
            var returnUrl = _configuration["VnPay:ReturnUrl"];
            var tmnCode = _configuration["VnPay:TmnCode"];
            var hashSecret = _configuration["VnPay:HashSecret"];

            vnpay.AddRequestData("vnp_Version", "2.1.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", tmnCode);
            vnpay.AddRequestData("vnp_Amount", (amount * 100).ToString("0"));
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", VnPayLibrary.GetIpAddress(HttpContext));
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang " + orderId);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", returnUrl);
            vnpay.AddRequestData("vnp_TxnRef", orderId.ToString());

            var paymentUrl = vnpay.CreateRequestUrl(url, hashSecret);
            return Ok(new { url = paymentUrl });
        }

        [HttpGet("return")]
        public async Task<IActionResult> PaymentReturn()
        {
            var vnpay = new VnPayLibrary();
            foreach (var (key, value) in Request.Query)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value.ToString());
                }
            }

            var vnp_orderId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
            var vnp_TransactionNo = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
            var vnp_SecureHash = Request.Query["vnp_SecureHash"].ToString();
            var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");

            var hashSecret = _configuration["VnPay:HashSecret"];

            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, hashSecret);

            if (checkSignature)
            {
                if (vnp_ResponseCode == "00")
                {
                    var order = await _context.Orders.FindAsync((int)vnp_orderId);
                    if (order != null)
                    {
                        order.Status = "Paid";
                        await _context.SaveChangesAsync();
                    }
                    return Ok(new { message = "Giao dịch thành công" });
                }
                else
                {
                    return BadRequest(new { message = "Có lỗi xảy ra trong quá trình xử lý" });
                }
            }
            else
            {
                return BadRequest(new { message = "Lỗi xác thực chữ ký" });
            }
        }
    }
}
