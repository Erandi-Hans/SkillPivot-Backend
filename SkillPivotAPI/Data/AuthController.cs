using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillPivotAPI.Data;
using SkillPivotAPI.Models;
using System.Net;
using System.Net.Mail;

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
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);
            if (user == null) return Unauthorized(new { message = "Invalid email or password." });

            string responseRole = user.Role == "Internship Seekers" ? "Intern" : user.Role;
            int? companyId = null;

            if (responseRole == "Company")
            {
                var company = await _context.Companies.FirstOrDefaultAsync(c => c.ContactEmail == user.Email);
                if (company != null) companyId = company.CompanyId;
            }

            return Ok(new
            {
                message = "Login successful!",
                user = new { userId = user.UserId, email = user.Email, role = responseRole, firstName = user.Firstname, lastName = user.Lastname, companyId = companyId },
                role = responseRole,
                companyId = companyId
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
                return BadRequest(new { message = "Email already registered." });

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            if (user.Role == "Internship Seekers" || user.Role == "Student")
            {
                var studentProfile = new Student
                {
                    UserId = user.UserId,
                    University = "",
                    Degree = "",
                    GPA = "",
                    Skills = "",
                    Gender = "",
                    IsVerified = false,
                    NicDocumentPath = ""
                };
                _context.Students.Add(studentProfile); // Students ලෙස නිවැරදි කරන ලදී
                await _context.SaveChangesAsync();
            }
            return Ok(new { message = "Registration successful!" });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null) return BadRequest(new { message = "No account associated with this email." });

            string otp = new Random().Next(1000, 9999).ToString();
            user.Verificationcode = otp;
            user.OTPExpiry = DateTime.Now.AddMinutes(10);
            await _context.SaveChangesAsync();

            try
            {
                // SMTP Setup
                return Ok(new { message = "Reset code has been sent to your email." });
            }
            catch
            {
                return StatusCode(500, new { message = "SMTP failed." });
            }
        }
    }

    public class LoginRequest { public string Email { get; set; } = string.Empty; public string Password { get; set; } = string.Empty; }
    public class ForgotPasswordDto { public string Email { get; set; } = string.Empty; }
}