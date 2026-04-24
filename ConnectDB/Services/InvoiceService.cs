using ConnectDB.Models;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System.IO;

namespace ConnectDB.Services
{
    public class InvoiceService
    {
        public byte[] GenerateInvoicePdf(Order order)
        {
            var document = new PdfDocument();
            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);
            var fontTitle = new XFont("Arial", 20, XFontStyle.Bold);
            var fontNormal = new XFont("Arial", 12, XFontStyle.Regular);
            var fontBold = new XFont("Arial", 12, XFontStyle.Bold);

            // Draw header
            gfx.DrawString("HOÁ ĐƠN THANH TOÁN", fontTitle, XBrushes.Black, new XRect(0, 40, page.Width, 40), XStringFormats.Center);
            
            // Draw order info
            gfx.DrawString($"Mã đơn hàng: {order.OrderId}", fontBold, XBrushes.Black, new XPoint(40, 100));
            gfx.DrawString($"Ngày đặt: {order.CreatedAt:dd/MM/yyyy HH:mm}", fontNormal, XBrushes.Black, new XPoint(40, 120));
            gfx.DrawString($"Trạng thái: {order.Status}", fontNormal, XBrushes.Black, new XPoint(40, 140));

            // Draw items table header
            int startY = 180;
            gfx.DrawRectangle(XBrushes.LightGray, new XRect(40, startY, page.Width - 80, 20));
            gfx.DrawString("Tên SP", fontBold, XBrushes.Black, new XPoint(50, startY + 15));
            gfx.DrawString("SL", fontBold, XBrushes.Black, new XPoint(300, startY + 15));
            gfx.DrawString("Đơn giá", fontBold, XBrushes.Black, new XPoint(350, startY + 15));
            gfx.DrawString("Thành tiền", fontBold, XBrushes.Black, new XPoint(450, startY + 15));

            startY += 30;

            // Draw items
            if (order.OrderItems != null)
            {
                foreach (var item in order.OrderItems)
                {
                    var productName = item.Product?.Name ?? "Sản phẩm";
                    var quantity = item.Quantity;
                    var price = item.PriceAtPurchase;
                    var total = quantity * price;

                    gfx.DrawString(productName.Length > 30 ? productName.Substring(0, 30) + "..." : productName, fontNormal, XBrushes.Black, new XPoint(50, startY));
                    gfx.DrawString(quantity.ToString(), fontNormal, XBrushes.Black, new XPoint(300, startY));
                    gfx.DrawString(price.ToString("N0") + "đ", fontNormal, XBrushes.Black, new XPoint(350, startY));
                    gfx.DrawString(total.ToString("N0") + "đ", fontNormal, XBrushes.Black, new XPoint(450, startY));

                    startY += 20;
                }
            }

            // Draw total
            startY += 20;
            gfx.DrawLine(XPens.Black, 40, startY, page.Width - 40, startY);
            startY += 20;
            gfx.DrawString("Tổng cộng:", fontBold, XBrushes.Black, new XPoint(350, startY));
            gfx.DrawString(order.FinalPrice.ToString("N0") + "đ", fontBold, XBrushes.Black, new XPoint(450, startY));

            using (var stream = new MemoryStream())
            {
                document.Save(stream, false);
                return stream.ToArray();
            }
        }
    }
}
