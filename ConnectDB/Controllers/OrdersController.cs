using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConnectDB.Models;
using ConnectDB.Services;

namespace ConnectDB.Controllers
{
    public class OrderUpdateDto
    {
        [System.Text.Json.Serialization.JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
    }

    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly InvoiceService _invoiceService;

        public OrdersController(AppDbContext context, InvoiceService invoiceService)
        {
            _context = context;
            _invoiceService = invoiceService;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            return Ok(await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Variant)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Variant)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
                return NotFound();

            return Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(Order order)
        {
            order.CreatedAt = DateTime.Now;
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Load full order details with products to return to frontend
            var fullOrder = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Variant)
                .FirstOrDefaultAsync(o => o.OrderId == order.OrderId);

            return Ok(fullOrder);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromQuery] string status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound();

            if (!string.IsNullOrEmpty(status))
            {
                order.Status = status;
                order.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }

            return Ok(order);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
                return NotFound();

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout(int userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
                return BadRequest("Cart is empty");

            decimal total = 0;

            var order = new Order
            {
                UserId = userId,
                Status = "Pending",
                CreatedAt = DateTime.Now,
                OrderItems = new List<OrderItem>()
            };

            foreach (var item in cart.CartItems)
            {
                var price = item.Product.SalePrice ?? item.Product.Price;

                total += price * item.Quantity;

                order.OrderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    VariantId = item.VariantId,
                    Quantity = item.Quantity,
                    PriceAtPurchase = price
                });
            }

            order.TotalPrice = total;
            order.FinalPrice = total;

            _context.Orders.Add(order);

            _context.CartItems.RemoveRange(cart.CartItems);

            await _context.SaveChangesAsync();

            // Load full order details with products to return to frontend
            var fullOrder = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Variant)
                .FirstOrDefaultAsync(o => o.OrderId == order.OrderId);

            return Ok(fullOrder);
        }

        [HttpGet("{id}/invoice")]
        public async Task<IActionResult> GetInvoice(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
                return NotFound();

            var pdfBytes = _invoiceService.GenerateInvoicePdf(order);
            return File(pdfBytes, "application/pdf", $"Invoice_{order.OrderId}.pdf");
        }

        [HttpPost("{id}/refund")]
        public async Task<IActionResult> RefundOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            if (order.Status != "Paid")
            {
                return BadRequest(new { message = "Chỉ có thể hoàn tiền đơn hàng đã thanh toán." });
            }

            // Gọi API hoàn tiền của VNPay thực tế ở đây. 
            // Demo: Tự động đổi trạng thái thành "Refunded"
            order.Status = "Refunded";
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã yêu cầu hoàn tiền thành công." });
        }
    }
}