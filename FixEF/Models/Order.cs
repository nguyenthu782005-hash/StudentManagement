using System.ComponentModel.DataAnnotations;

namespace ConnectDB.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        public decimal TotalPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal FinalPrice { get; set; }

        public string Status { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;

        public string Note { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public int UserId { get; set; }

        public User User { get; set; } = null!;

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}