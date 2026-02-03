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

        // --- 1. LOGIN: Handles authentication, role mapping, and company ID retrieval ---
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            // Fetch user by email and password from SQL Database
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            // ROLE MAPPING FIX:
            string responseRole = user.Role;
            if (responseRole == "Internship Seekers")
            {
                responseRole = "Intern";
            }

            // NEW LOGIC: Retrieve CompanyId if the user belongs to a company
            int? companyId = null;
            if (responseRole == "Company")
            {
                // Match the user's email with the ContactEmail in the Companies table
                var company = await _context.Companies.FirstOrDefaultAsync(c => c.ContactEmail == user.Email);
                if (company != null)
                {
                    companyId = company.CompanyId;
                }
            }

            // Return data with companyId included in the response
            return Ok(new
            {
                message = "Login successful!",
                user = new
                {
                    userId = user.UserId,
                    email = user.Email,
                    role = responseRole,
                    firstName = user.Firstname,
                    lastName = user.Lastname,
                    // Sending companyId here so frontend can store it
                    companyId = companyId
                },
                role = responseRole,
                // Also sending companyId at top level for easier access
                companyId = companyId
            });
        }

        // --- 2. REGISTER: Modified to include Student profile creation ---
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
            {
                return BadRequest(new { message = "Email already registered." });
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // NEW LOGIC: If role is Student or Internship Seeker, create a record in Students table
            if (user.Role == "Internship Seekers" || user.Role == "Student")
            {
                var studentProfile = new Student
                {
                    UserId = user.UserId,
                    University = "",
                    Degree = "",
                    GPA = "",
                    Skills = ""
                };
                _context.Students.Add(studentProfile);
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = "Registration successful!" });
        }

        // --- 3. FORGOT PASSWORD: Gmail SMTP Integration ---
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
                string senderEmail = "erandi2287hansika@gmail.com";
                string appPassword = "dqznegmyqlvhplaj";

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail, "SkillPivot Support"),
                    Subject = "Your Password Reset OTP",
                    Body = $"Your reset code is: {otp}. Valid for 10 mins.",
                    IsBodyHtml = false
                };
                mailMessage.To.Add(user.Email);

                using (var smtpClient = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential(senderEmail, appPassword);
                    smtpClient.EnableSsl = true;
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    await smtpClient.SendMailAsync(mailMessage);
                }
                return Ok(new { message = "Reset code has been sent to your email." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Email delivery failed. Update your Google App Password." });
            }
        }
    }

    public class LoginRequest { public string Email { get; set; } = string.Empty; public string Password { get; set; } = string.Empty; }
    public class ForgotPasswordDto { public string Email { get; set; } = string.Empty; }
}