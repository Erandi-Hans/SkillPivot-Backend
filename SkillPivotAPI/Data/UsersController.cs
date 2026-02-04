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
        private readonly IWebHostEnvironment _environment; // Added to handle server file paths

        /// <summary>
        /// Constructor: Injects the ApplicationDbContext and IWebHostEnvironment.
        /// </summary>
        public UsersController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment; // Injected for static file management
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
                else
                {
                    throw;
                }
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
        /// Endpoint: POST api/Users/upload-image/{id}
        /// </summary>
        /// <param name="id">User ID to associate the image with.</param>
        /// <param name="profileImage">The image file sent from the frontend.</param>
        [HttpPost("upload-image/{id}")]
        public async Task<IActionResult> UploadImage(int id, IFormFile profileImage)
        {
            if (profileImage == null || profileImage.Length == 0)
            {
                return BadRequest(new { message = "No image file provided." });
            }

            // Define the directory path where images will be stored
            string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");

            // Ensure the directory exists
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Generate a unique file name to prevent overwriting
            string uniqueFileName = $"profile_{id}_{Guid.NewGuid()}{Path.GetExtension(profileImage.FileName)}";
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Save the file to the physical storage (wwwroot/uploads)
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await profileImage.CopyToAsync(stream);
            }

            // Retrieve the user from the database to update the ProfilePicture path
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound(new { message = "User not found." });

            user.ProfilePicture = uniqueFileName;
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Profile picture updated successfully!", fileName = uniqueFileName });
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }

    public class ChangePasswordDto
    {
        public int UserId { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}