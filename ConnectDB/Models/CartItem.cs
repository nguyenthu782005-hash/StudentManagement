using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ConnectDB.Models
{
    public class CartItem
    {
        [Key]
        public int CartItemId { get; set; }

        public int Quantity { get; set; }

        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int? VariantId { get; set; }

        [JsonIgnore]
        public Cart? Cart { get; set; }

        [JsonIgnore]
        public Product? Product { get; set; }

        [JsonIgnore]
        public ProductVariant? Variant { get; set; }
    }
}