using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ConnectDB.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;

        public string Role { get; set; } = "customer";
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Password Reset
        public string? ResetToken { get; set; }
        public DateTime? ResetTokenExpiry { get; set; }

        [JsonIgnore]
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        [JsonIgnore]
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
