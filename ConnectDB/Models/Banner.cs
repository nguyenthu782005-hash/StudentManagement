using System.ComponentModel.DataAnnotations;

namespace ConnectDB.Models
{
    public class Banner
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string ImageUrl { get; set; } = string.Empty;

        public string LinkUrl { get; set; } = string.Empty;

        public int Priority { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
