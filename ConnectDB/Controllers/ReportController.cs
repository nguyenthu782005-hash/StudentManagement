using ClosedXML.Excel;
using ConnectDB.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConnectDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Staff")]
    public class ReportController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReportController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("revenue")]
        public async Task<IActionResult> ExportRevenue()
        {
            var orders = await _context.Orders
                .Where(o => o.Status == "Completed" || o.Status == "Paid")
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Doanh Thu");
                var currentRow = 1;

                worksheet.Cell(currentRow, 1).Value = "Mã Đơn Hàng";
                worksheet.Cell(currentRow, 2).Value = "Khách Hàng ID";
                worksheet.Cell(currentRow, 3).Value = "Ngày Đặt";
                worksheet.Cell(currentRow, 4).Value = "Tổng Tiền";
                worksheet.Cell(currentRow, 5).Value = "Trạng Thái";

                foreach (var order in orders)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = order.OrderId;
                    worksheet.Cell(currentRow, 2).Value = order.UserId;
                    worksheet.Cell(currentRow, 3).Value = order.CreatedAt.ToString("dd/MM/yyyy HH:mm");
                    worksheet.Cell(currentRow, 4).Value = order.FinalPrice;
                    worksheet.Cell(currentRow, 5).Value = order.Status;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BaoCaoDoanhThu.xlsx");
                }
            }
        }

        [HttpGet("products")]
        public async Task<IActionResult> ExportTopProducts()
        {
            var products = await _context.Products
                .OrderByDescending(p => p.Views)
                .Select(p => new {
                    p.ProductId,
                    p.Name,
                    p.Price,
                    p.Stock,
                    p.Views
                })
                .ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sản Phẩm");
                var currentRow = 1;

                worksheet.Cell(currentRow, 1).Value = "Mã SP";
                worksheet.Cell(currentRow, 2).Value = "Tên Sản Phẩm";
                worksheet.Cell(currentRow, 3).Value = "Giá";
                worksheet.Cell(currentRow, 4).Value = "Tồn Kho";
                worksheet.Cell(currentRow, 5).Value = "Lượt Xem";

                foreach (var prod in products)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = prod.ProductId;
                    worksheet.Cell(currentRow, 2).Value = prod.Name;
                    worksheet.Cell(currentRow, 3).Value = prod.Price;
                    worksheet.Cell(currentRow, 4).Value = prod.Stock;
                    worksheet.Cell(currentRow, 5).Value = prod.Views;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BaoCaoSanPham.xlsx");
                }
            }
        }
    }
}
