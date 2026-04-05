using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ConnectDB.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;

        public decimal Price { get; set; }
        public decimal? SalePrice { get; set; }

        public string Description { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;

        public int Stock { get; set; }
        public int Views { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int CategoryId { get; set; }
        public int? BrandId { get; set; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        public Brand? Brand { get; set; }

        [JsonIgnore]
        public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();

        [JsonIgnore]
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}