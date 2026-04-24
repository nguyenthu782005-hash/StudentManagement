using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConnectDB.Models;
using ConnectDB.Services;

namespace ConnectDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;

        public UsersController(AppDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            return Ok(await _context.Users.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(User user)
        {
            var exists = await _context.Users
                .AnyAsync(u => u.Email == user.Email);

            if (exists)
                return BadRequest("Email already exists");

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.Password == password);

            if (user == null)
                return Unauthorized("Invalid email or password");

            return Ok(user);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
                return NotFound("Email khong ton tai trong he thong.");

            // Generate 6-digit OTP
            var otp = new Random().Next(100000, 999999).ToString();
            user.ResetToken = otp;
            user.ResetTokenExpiry = DateTime.UtcNow.AddMinutes(10);
            await _context.SaveChangesAsync();

            // Send email
            var emailBody = $@"
                <div style='font-family: Arial, sans-serif; max-width: 500px; margin: 0 auto; padding: 40px 20px;'>
                    <div style='text-align: center; margin-bottom: 30px;'>
                        <h1 style='color: #6c5ce7; margin: 0;'>THU<span style='color: #fd79a8;'>STORE</span></h1>
                    </div>
                    <div style='background: #f8fafc; border-radius: 16px; padding: 30px; text-align: center;'>
                        <h2 style='margin-top: 0; color: #1a1a2e;'>Dat lai mat khau</h2>
                        <p style='color: #64748b;'>Ma xac nhan cua ban la:</p>
                        <div style='background: white; border: 2px dashed #6c5ce7; border-radius: 12px; padding: 20px; margin: 20px 0;'>
                            <span style='font-size: 32px; font-weight: 800; letter-spacing: 8px; color: #6c5ce7;'>{otp}</span>
                        </div>
                        <p style='color: #94a3b8; font-size: 14px;'>Ma nay se het han sau 10 phut.</p>
                        <p style='color: #94a3b8; font-size: 14px;'>Neu ban khong yeu cau dat lai mat khau, hay bo qua email nay.</p>
                    </div>
                </div>";

            try
            {
                await _emailService.SendEmailAsync(user.Email, "THU STORE - Ma dat lai mat khau", emailBody);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email send error: {ex.Message}");
                return StatusCode(500, "Khong the gui email. Vui long thu lai sau.");
            }

            return Ok(new { message = "Ma xac nhan da duoc gui den email cua ban." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
                return NotFound("Email khong ton tai.");

            if (user.ResetToken != request.Otp || user.ResetTokenExpiry < DateTime.UtcNow)
                return BadRequest("Ma OTP khong hop le hoac da het han.");

            user.Password = request.NewPassword;
            user.ResetToken = null;
            user.ResetTokenExpiry = null;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Mat khau da duoc dat lai thanh cong." });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, User user)
        {
            if (id != user.UserId)
                return BadRequest();

            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("{id}/lock")]
        public async Task<IActionResult> LockUser(int id, [FromBody] bool isActive)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.IsActive = isActive;
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        [HttpPut("{id}/role")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] string role)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            if (role != "Admin" && role != "Staff" && role != "customer")
            {
                return BadRequest("Role khong hop le.");
            }

            user.Role = role;
            await _context.SaveChangesAsync();

            return Ok(user);
        }
    }

    // Request DTOs
    public class ForgotPasswordRequest
    {
        public string Email { get; set; } = string.Empty;
    }

    public class ResetPasswordRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
