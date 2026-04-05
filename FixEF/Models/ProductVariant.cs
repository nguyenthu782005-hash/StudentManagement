using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConnectDB.Models
{
    public class ProductVariant
    {
        [Key]
        public int VariantId { get; set; }

        public string Size { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;

        public int Stock { get; set; }
        public decimal? Price { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; } = null!;
    }
}