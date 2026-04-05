using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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

        [JsonIgnore]
        public Product? Product { get; set; }

        [JsonIgnore]
        public ProductVariant? Variant { get; set; }

        [JsonIgnore]
        public Order? Order { get; set; }
    }
}