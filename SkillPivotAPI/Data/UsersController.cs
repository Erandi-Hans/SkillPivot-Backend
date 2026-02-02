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

        /// <summary>
        /// Constructor to initialize the database context.
        /// </summary>
        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves the personal profile details of a user by their ID.
        /// GET: api/Users/5
        /// </summary>
        /// <param name="id">The unique ID of the user.</param>
        /// <returns>User object containing profile data.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            // Fetch user from the database using the provided ID
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound(new { message = "User profile not found." });
            }

            // Returns the user object (includes Firstname, Lastname, Email, etc.)
            return Ok(user);
        }

        /// <summary>
        /// Updates the profile information for a specific user.
        /// PUT: api/Users/5
        /// </summary>
        /// <param name="id">The ID of the user to update.</param>
        /// <param name="updatedUser">The updated user data from the frontend.</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, [FromBody] User updatedUser)
        {
            if (id != updatedUser.UserId)
            {
                return BadRequest(new { message = "User ID mismatch." });
            }

            // Find the existing user record in the database
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // --- FIXED: Mapping properties according to your SQL Database schema ---
            user.Firstname = updatedUser.Firstname;
            user.Lastname = updatedUser.Lastname;
            user.Email = updatedUser.Email;

            // Mark the entity as modified so Entity Framework knows what to update
            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id)) return NotFound();
                else throw;
            }

            return Ok(new { message = "Profile updated successfully!", user });
        }

        /// <summary>
        /// Helper method to verify if a user exists in the database.
        /// </summary>
        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}