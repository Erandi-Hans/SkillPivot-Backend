using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillPivotAPI.Data;
using SkillPivotAPI.Models;

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
            // Check if the email already exists in the database
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
            {
                return BadRequest(new { message = "Email already registered." });
            }

            // Add the new user object to the Users DbSet
            _context.Users.Add(user);

            // Save changes to the SQL database. 
            // Note: This will fail if the database columns (like Firstname) don't match the model.
            await _context.SaveChangesAsync();

            return Ok(new { message = "User registered successfully!" });
        }

        // --- 2. LOGIN ---
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Find the user with the matching email
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            // Verify if user exists and password matches
            if (user == null || user.Password != request.Password)
            {
                return Unauthorized(new { message = "Invalid Email or Password" });
            }

            // Return user details upon successful login
            return Ok(new
            {
                id = user.UserId,
                email = user.Email,
                role = user.Role
            });
        }
    }
}