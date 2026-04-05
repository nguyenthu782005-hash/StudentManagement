using System.ComponentModel.DataAnnotations;

namespace ConnectDB.Models
{
    public class Brand
    {
        [Key]
        public int BrandId { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Logo { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}