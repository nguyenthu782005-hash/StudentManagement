using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using ConnectDB.Models;

namespace ConnectDB.Models
{
    public class Brand
    {
        [Key]
        public int BrandId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(255)]
        public string Logo { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [JsonIgnore]
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}