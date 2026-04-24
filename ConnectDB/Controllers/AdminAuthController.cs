using ConnectDB.Models;
using ConnectDB.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace ConnectDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminAuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwtService;

        public AdminAuthController(AppDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public class LoginRequest
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            bool isPasswordValid = false;

            if (user != null)
            {
                try
                {
                    isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
                }
                catch (BCrypt.Net.SaltParseException)
                {
                    // Fallback for plain text passwords during development
                    isPasswordValid = (user.Password == request.Password);
                }
            }

            if (user == null || !isPasswordValid)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            if (!user.IsActive)
            {
                return Unauthorized(new { message = "Account is locked" });
            }

            if (user.Role != "Admin" && user.Role != "Staff")
            {
                return Unauthorized(new { message = "Access denied. Admin or Staff only." });
            }

            var token = _jwtService.GenerateToken(user);

            return Ok(new
            {
                Token = token,
                User = new { user.UserId, user.Name, user.Email, user.Role }
            });
        }
    }
}
