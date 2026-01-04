using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillPivotAPI.Data;
using SkillPivotAPI.Models; // මෙය අනිවාර්යයෙන්ම අවශ්‍යයි

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

        // --- 1. REGISTER (SIGN-UP) ---
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            // ඊමේල් එක දැනටමත් තියෙනවද බලනවා
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
            {
                return BadRequest(new { message = "Email already registered." });
            }

            // User ව database එකට ඇතුළත් කරනවා
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User registered successfully!" });
        }

        // --- 2. LOGIN ---
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || user.Password != request.Password)
            {
                return Unauthorized(new { message = "Invalid Email or Password" });
            }

            return Ok(new
            {
                id = user.UserId,
                email = user.Email,
                role = user.Role
            });
        }
    }
}