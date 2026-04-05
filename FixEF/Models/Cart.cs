using System.ComponentModel.DataAnnotations;

namespace ConnectDB.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }

        public int? UserId { get; set; }

        public string SessionId { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}