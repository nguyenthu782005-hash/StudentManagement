using System.ComponentModel.DataAnnotations;

namespace ConnectDB.Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }

        public int Rating { get; set; }

        public string Comment { get; set; } = string.Empty;

        public bool IsApproved { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int ProductId { get; set; }
        public int UserId { get; set; }

        public Product Product { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}