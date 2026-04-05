using System.ComponentModel.DataAnnotations;

namespace ConnectDB.Models
{
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }

        public int Quantity { get; set; }
        public decimal PriceAtPurchase { get; set; }

        public int ProductId { get; set; }
        public int? VariantId { get; set; }
        public int OrderId { get; set; }
        public Product Product { get; set; } = null!;
        public ProductVariant? Variant { get; set; }
        public Order Order { get; set; } = null!;
    }
}