using Microsoft.AspNetCore.Mvc;

namespace ConnectDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public UploadController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpPost("product")]
        public async Task<IActionResult> UploadProductImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var webRoot = string.IsNullOrEmpty(_env.WebRootPath) ? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot") : _env.WebRootPath;
            var uploadsFolder = Path.Combine(webRoot, "uploads", "products");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return the relative path for the frontend to use
            var relativePath = $"/uploads/products/{fileName}";
            return Ok(new { url = relativePath });
        }

        [HttpPost("brand")]
        public async Task<IActionResult> UploadBrandImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var webRoot = string.IsNullOrEmpty(_env.WebRootPath) ? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot") : _env.WebRootPath;
            var uploadsFolder = Path.Combine(webRoot, "uploads", "brands");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return the relative path for the frontend to use
            var relativePath = $"/uploads/brands/{fileName}";
            return Ok(new { url = relativePath });
        }
    }
}
