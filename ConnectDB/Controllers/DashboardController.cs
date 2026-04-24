using ConnectDB.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace ConnectDB.Controllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Staff")]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;

        public DashboardController(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboardStats()
        {
            if (!_cache.TryGetValue("DashboardStats", out object? stats))
            {
                var today = DateTime.Today;
                var startOfMonth = new DateTime(today.Year, today.Month, 1);

                var todayRevenue = await _context.Orders
                    .Where(o => (o.Status == "Completed" || o.Status == "Paid") && o.CreatedAt >= today)
                    .SumAsync(o => (decimal?)o.FinalPrice) ?? 0;

                var monthRevenue = await _context.Orders
                    .Where(o => (o.Status == "Completed" || o.Status == "Paid") && o.CreatedAt >= startOfMonth)
                    .SumAsync(o => (decimal?)o.FinalPrice) ?? 0;
                    
                var totalRevenue = await _context.Orders
                    .Where(o => o.Status == "Completed" || o.Status == "Paid")
                    .SumAsync(o => (decimal?)o.FinalPrice) ?? 0;

                var newOrders = await _context.Orders
                    .Where(o => o.CreatedAt >= today.AddDays(-1))
                    .CountAsync();
                    
                var totalOrders = await _context.Orders.CountAsync();

                var lowStockProducts = await _context.Products
                    .Where(p => p.Stock < 5)
                    .CountAsync();
                    
                var totalProducts = await _context.Products.CountAsync();

                var newCustomers = await _context.Users
                    .Where(u => u.Role == "customer" && u.CreatedAt >= today.AddDays(-7))
                    .CountAsync();
                    
                var totalCustomers = await _context.Users
                    .Where(u => u.Role == "customer")
                    .CountAsync();

                stats = new
                {
                    TodayRevenue = todayRevenue,
                    MonthRevenue = monthRevenue,
                    TotalRevenue = totalRevenue,
                    NewOrders = newOrders,
                    TotalOrders = totalOrders,
                    LowStockProducts = lowStockProducts,
                    TotalProducts = totalProducts,
                    NewCustomers = newCustomers,
                    TotalCustomers = totalCustomers
                };

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

                _cache.Set("DashboardStats", stats, cacheOptions);
            }

            return Ok(stats);
        }
    }
}
