using System.ComponentModel.DataAnnotations;

namespace ConnectDB.Models
{
    public class Wishlist
    {
        [Key]
        public int WishlistId { get; set; }

        public int UserId { get; set; }
        public int ProductId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public User User { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}