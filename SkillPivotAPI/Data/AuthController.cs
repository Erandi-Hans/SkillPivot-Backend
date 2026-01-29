using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillPivotAPI.Data;
using SkillPivotAPI.Models;
using Google.Apis.Auth; // Required for Google Authentication
using Newtonsoft.Json;   // Required for JSON data handling

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
            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
            {
                return BadRequest(new { message = "Email already registered." });
            }

            var user = new User
            {
                Email = model.Email,
                Password = model.Password,
                Role = model.Role,
                Firstname = model.Role == "Company" ? model.CompanyName : model.Firstname,
                Lastname = model.Role == "Company" ? "Company" : model.Lastname
            };

            _context.Users.Add(user);

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

            await _context.SaveChangesAsync();
            return Ok(new { message = "Registration successful!" });
        }

        // --- 2. LOGIN (STANDARD EMAIL/PASSWORD) ---
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

        // --- 3. GOOGLE LOGIN ---
        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            try
            {
                var httpClient = new HttpClient();
                var response = await httpClient.GetAsync($"https://www.googleapis.com/oauth2/v3/userinfo?access_token={request.Token}");

                if (!response.IsSuccessStatusCode)
                    return BadRequest(new { message = "Invalid Google Token" });

                var content = await response.Content.ReadAsStringAsync();
                var googleUser = JsonConvert.DeserializeObject<dynamic>(content);

                string email = googleUser.email;
                string firstName = googleUser.given_name ?? "Google";
                string lastName = googleUser.family_name ?? "User";

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    user = new User
                    {
                        Email = email,
                        Firstname = firstName,
                        Lastname = lastName,
                        Role = "Internship Seeker",
                        Password = "GoogleUser_" + Guid.NewGuid().ToString().Substring(0, 8),
                        IsVerified = true
                    };

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                }

                return Ok(new
                {
                    id = user.UserId,
                    email = user.Email,
                    role = user.Role,
                    firstName = user.Firstname
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal Server Error", details = ex.Message });
            }
        }

        // --- 4. FORGOT PASSWORD (Generate & Save OTP) ---
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null)
            {
                return BadRequest(new { message = "No account associated with this email." });
            }

            // Generate random 4-digit OTP code
            string otp = new Random().Next(1000, 9999).ToString();

            // Store OTP and set 10-minute expiry
            user.Verificationcode = otp;
            user.OTPExpiry = DateTime.Now.AddMinutes(10);

            await _context.SaveChangesAsync();

            // Print to debug console
            Console.WriteLine($"[DEBUG] OTP for {user.Email} is: {otp}");

            return Ok(new { message = "Reset code sent to your email." });
        }

        // --- 5. VERIFY OTP ---
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null)
                return BadRequest(new { message = "User not found." });

            // Verify if OTP matches and has not expired
            if (user.Verificationcode == model.Code && user.OTPExpiry > DateTime.Now)
            {
                return Ok(new { message = "OTP verified successfully." });
            }

            return BadRequest(new { message = "Invalid or expired OTP code." });
        }

        // --- 6. RESET PASSWORD (Update New Password) ---
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null)
                return BadRequest(new { message = "User not found." });

            // Save new password and clear the OTP fields
            user.Password = model.NewPassword;
            user.Verificationcode = null;
            user.OTPExpiry = null;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Password has been reset successfully!" });
        }
    }

    // --- DATA TRANSFER OBJECTS (DTOs) ---

    public class GoogleLoginRequest
    {
        public string Token { get; set; } = string.Empty;
    }

    public class ForgotPasswordDto
    {
        public string Email { get; set; } = string.Empty;
    }

    public class VerifyOtpDto
    {
        public string Email { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }

    public class ResetPasswordDto
    {
        public string Email { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}