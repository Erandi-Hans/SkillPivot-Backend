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
        /// Constructor to initialize the database context for database operations.
        /// </summary>
        /// <param name="context">The application database context instance.</param>
        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves the personal profile details of a user by their unique ID.
        /// GET: api/Users/5
        /// </summary>
        /// <param name="id">The unique ID of the user to fetch.</param>
        /// <returns>A Task representing the asynchronous operation, containing the User object.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            // Search for the user record in the Users table by primary key
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                // Return 404 Not Found if no user exists with the given ID
                return NotFound(new { message = "User profile not found." });
            }

            // Return 200 OK with the full user object (Firstname, Lastname, Email, etc.)
            return Ok(user);
        }

        /// <summary>
        /// Updates the profile information for a specific user based on the provided ID.
        /// PUT: api/Users/5
        /// </summary>
        /// <param name="id">The unique ID of the user to be updated.</param>
        /// <param name="updatedUser">The modified user object received from the frontend.</param>
        /// <returns>A Task representing the asynchronous operation result.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, [FromBody] User updatedUser)
        {
            // Validate that the ID in the route matches the ID in the request body
            if (id != updatedUser.UserId)
            {
                return BadRequest(new { message = "User ID mismatch." });
            }

            // Find the existing user entity to update
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // --- MAPPING PROPERTIES: Synchronizing existing record with new data from frontend ---
            user.Firstname = updatedUser.Firstname;
            user.Lastname = updatedUser.Lastname;
            user.Email = updatedUser.Email;
            user.Location = updatedUser.Location; // Ensure location is updated
            user.Industry = updatedUser.Industry; // Ensure industry is updated

            // Mark the entity state as Modified for Entity Framework change tracking
            _context.Entry(user).State = EntityState.Modified;

            try
            {
                // Commit changes asynchronously to the SQL Server database
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Handle cases where the record might have been deleted or changed by another process
                if (!UserExists(id)) return NotFound();
                else throw;
            }

            // Return success response with the updated user data
            return Ok(new { message = "Profile updated successfully!", user });
        }

        /// <summary>
        /// Internal helper method to check if a user exists in the database.
        /// </summary>
        /// <param name="id">The unique ID of the user.</param>
        /// <returns>True if the user exists, otherwise false.</returns>
        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}