using System.ComponentModel.DataAnnotations;

namespace ConnectDB.Models
{
    public class BlogPost
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public string ThumbnailUrl { get; set; } = string.Empty;

        public bool IsPublished { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
