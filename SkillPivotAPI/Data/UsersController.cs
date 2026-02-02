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
        /// Constructor: Injects the ApplicationDbContext to handle all database interactions.
        /// </summary>
        /// <param name="context">The database context instance for the SkillPivot application.</param>
        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// HTTP GET: Retrieves a specific user's profile details using their Unique ID.
        /// Endpoint: GET api/Users/{id}
        /// </summary>
        /// <param name="id">The integer ID of the user to retrieve.</param>
        /// <returns>Returns the User object if found; otherwise, a 404 Not Found response.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            // Use Entity Framework to find the user in the database asynchronously
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                // Return a descriptive 404 error if the user record does not exist
                return NotFound(new { message = "User profile not found." });
            }

            // Return a 200 OK response containing the full user object
            return Ok(user);
        }

        /// <summary>
        /// HTTP PUT: Updates an existing user's profile information.
        /// Endpoint: PUT api/Users/{id}
        /// </summary>
        /// <param name="id">The ID of the user specified in the URL route.</param>
        /// <param name="updatedUser">The updated user data object sent from the frontend.</param>
        /// <returns>Returns 200 OK with a success message or an appropriate error code.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, [FromBody] User updatedUser)
        {
            // Security check: Ensure the ID in the URL matches the UserId in the request body
            if (id != updatedUser.UserId)
            {
                return BadRequest(new { message = "Request mismatch: User ID in URL and body must be the same." });
            }

            // Retrieve the existing user entity from the database context
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "Update failed: User record not found." });
            }

            // --- DATA MAPPING: Synchronizing the database record with modified values from frontend ---
            user.Firstname = updatedUser.Firstname;
            user.Lastname = updatedUser.Lastname;
            user.Email = updatedUser.Email;
            user.Location = updatedUser.Location; // Updates the geographical location
            user.Industry = updatedUser.Industry; // Updates the professional industry sector

            // Inform Entity Framework that the state of this entity has been modified
            _context.Entry(user).State = EntityState.Modified;

            try
            {
                // Save all pending changes to the SQL database
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Handle cases where multiple users might be editing the same record simultaneously
                if (!UserExists(id))
                {
                    return NotFound(new { message = "Update error: User no longer exists." });
                }
                else
                {
                    throw; // Re-throw the exception if it's a different concurrency issue
                }
            }

            // Return success response with a JSON object for frontend confirmation
            return Ok(new { message = "Profile updated successfully!", updatedData = user });
        }

        /// <summary>
        /// Helper Method: Checks the database to see if a specific user exists.
        /// </summary>
        /// <param name="id">The ID of the user to check.</param>
        /// <returns>True if the user exists; otherwise, false.</returns>
        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}