using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillPivotAPI.Data;
using SkillPivotAPI.Models;

namespace SkillPivotAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public UsersController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        /// <summary>
        /// HTTP GET: Retrieves all users from the database.
        /// Useful for the Admin Management table to show Students and Companies.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            // Returns the full list of users to be filtered on the frontend
            return await _context.Users.ToListAsync();
        }

        /// <summary>
        /// HTTP GET: Retrieves a specific user's profile details using their Unique ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "User profile not found." });
            }
            return Ok(user);
        }

        /// <summary>
        /// HTTP POST: Specifically for registering the system Administrator.
        /// Use this to register the email: 2021icts083@stu.vau.ac.lk
        /// </summary>
        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] User adminData)
        {
            // Check if the email is already registered to prevent duplicates
            if (await _context.Users.AnyAsync(u => u.Email == adminData.Email))
            {
                return BadRequest(new { message = "Email is already registered." });
            }

            // Manually force the role to "Admin" for security
            adminData.Role = "Admin";

            _context.Users.Add(adminData);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Admin account created successfully!" });
        }

        /// <summary>
        /// HTTP PUT: Updates an existing user's profile information.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, [FromBody] User updatedUser)
        {
            if (id != updatedUser.UserId)
            {
                return BadRequest(new { message = "Request mismatch: User ID in URL and body must be the same." });
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "Update failed: User record not found." });
            }

            user.Firstname = updatedUser.Firstname;
            user.Lastname = updatedUser.Lastname;
            user.Email = updatedUser.Email;
            user.Location = updatedUser.Location;
            user.Industry = updatedUser.Industry;

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound(new { message = "Update error: User no longer exists." });
                }
                else { throw; }
            }

            return Ok(new { message = "Profile updated successfully!", updatedData = user });
        }

        /// <summary>
        /// HTTP POST: Changes the user's password after verifying the current one.
        /// </summary>
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
        {
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            if (user.Password != request.CurrentPassword)
            {
                return BadRequest(new { message = "The current password you entered is incorrect." });
            }

            user.Password = request.NewPassword;
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Password updated successfully!" });
        }

        /// <summary>
        /// HTTP POST: Uploads and updates the user's profile picture.
        /// </summary>
        [HttpPost("upload-image/{id}")]
        public async Task<IActionResult> UploadImage(int id, IFormFile profileImage)
        {
            if (profileImage == null || profileImage.Length == 0)
            {
                return BadRequest(new { message = "No image file provided." });
            }

            string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = $"profile_{id}_{Guid.NewGuid()}{Path.GetExtension(profileImage.FileName)}";
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await profileImage.CopyToAsync(stream);
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound(new { message = "User not found." });

            user.ProfilePicture = uniqueFileName;
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Profile picture updated successfully!", fileName = uniqueFileName });
        }

        /// <summary>
        /// HTTP DELETE: Permanently removes a user from the system.
        /// Used by the Admin to delete spam or inactive accounts.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User deleted successfully." });
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }

    public class ChangePasswordDto
    {
        public int UserId { get; set; }
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}