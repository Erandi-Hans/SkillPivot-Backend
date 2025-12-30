using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillPivotAPI.Data;
//using SkillPivotAPI.Models;

namespace SkillPivotAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // 1. Database එකේ මේ email එක තියෙනවද බලනවා
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            // 2. User කෙනෙක් නැත්නම් හෝ Password එක වැරදි නම් Error එකක් දෙනවා
            if (user == null || user.Password != request.Password)
            {
                return Unauthorized(new { message = "Invalid Email or Password" });
            }

            // 3. සාර්ථක නම් User ගේ තොරතුරු React එකට යවනවා
            return Ok(new
            {
                id = user.UserId,
                email = user.Email,
                role = user.Role
            });
        }
    }
}