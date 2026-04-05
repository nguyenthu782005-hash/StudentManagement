using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConnectDB.Models;

namespace ConnectDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CouponsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetCoupons()
        {
            return Ok(await _context.Coupons.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCoupon(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);

            if (coupon == null)
                return NotFound();

            return Ok(coupon);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCoupon(Coupon coupon)
        {
            _context.Coupons.Add(coupon);
            await _context.SaveChangesAsync();

            return Ok(coupon);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCoupon(int id, Coupon coupon)
        {
            if (id != coupon.CouponId)
                return BadRequest();

            _context.Entry(coupon).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(coupon);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCoupon(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);

            if (coupon == null)
                return NotFound();

            _context.Coupons.Remove(coupon);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("apply")]
        public async Task<IActionResult> ApplyCoupon(string code, decimal orderTotal)
        {
            var coupon = await _context.Coupons
                .FirstOrDefaultAsync(c => c.Code == code);

            if (coupon == null)
                return BadRequest("Invalid coupon");

            if (coupon.ExpiryDate < DateTime.Now)
                return BadRequest("Coupon expired");

            if (coupon.UsedCount >= coupon.MaxUses)
                return BadRequest("Coupon limit reached");

            if (orderTotal < coupon.MinOrder)
                return BadRequest("Order not eligible");

            decimal discount = 0;

            if (coupon.DiscountType == "percentage")
                discount = orderTotal * coupon.DiscountValue / 100;
            else
                discount = coupon.DiscountValue;

            return Ok(new
            {
                discount,
                finalTotal = orderTotal - discount
            });
        }
    }
}