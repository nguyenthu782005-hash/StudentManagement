using System.ComponentModel.DataAnnotations;

namespace ConnectDB.Models
{
    public class Coupon
    {
        [Key]
        public int CouponId { get; set; }

        public string Code { get; set; } = string.Empty;

        public decimal DiscountValue { get; set; }
        public string DiscountType { get; set; } = string.Empty; 

        public decimal MinOrder { get; set; }

        public int MaxUses { get; set; }
        public int UsedCount { get; set; }

        public DateTime ExpiryDate { get; set; }
    }
}