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
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            // Check if the email already exists in the Users table
            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
            {
                return BadRequest(new { message = "Email already registered." });
            }

            // Create a new User object
            // If the role is 'Company', we use CompanyName as Firstname to avoid NULL database errors
            var user = new User
            {
                Email = model.Email,
                Password = model.Password,
                Role = model.Role,
                Firstname = model.Role == "Company" ? model.CompanyName : model.Firstname,
                Lastname = model.Role == "Company" ? "Company" : model.Lastname
            };

            // Add the user to the Users DbSet
            _context.Users.Add(user);

            // If the registered role is 'Company', create a corresponding profile in the Companies table
            if (model.Role == "Company")
            {
                var company = new Company
                {
                    CompanyName = model.CompanyName ?? "New Company",
                    ContactEmail = model.Email,
                    RegisteredDate = DateTime.Now,
                    IsVerified = false,
                    Industry = "Not Specified"
                };
                _context.Companies.Add(company);
            }

            // Save all changes to the database
            await _context.SaveChangesAsync();

            return Ok(new { message = "Registration successful!" });
        }

        // --- 2. LOGIN ---
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Find the user by their email address
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            // Validate if the user exists and if the password matches
            if (user == null || user.Password != request.Password)
            {
                return Unauthorized(new { message = "Invalid Email or Password" });
            }

            // Return basic user details upon successful authentication
            return Ok(new
            {
                id = user.UserId,
                email = user.Email,
                role = user.Role
            });
        }
    }
}