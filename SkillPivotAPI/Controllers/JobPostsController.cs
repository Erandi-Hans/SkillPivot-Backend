using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillPivotAPI.Data; // Access to ApplicationDbContext
using SkillPivotAPI.Models; // Access to JobPost Model

namespace SkillPivotAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobPostsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // Constructor: Injecting the Database Context
        public JobPostsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates a new job post in the database.
        /// Route: POST /api/JobPosts
        /// </summary>
        /// <param name="jobPost">The job post object received from the Frontend</param>
        [HttpPost]
        public async Task<IActionResult> PostJob([FromBody] JobPost jobPost)
        {
            // Check if the model state is valid according to the JobPost class rules
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if the incoming data is null
            if (jobPost == null)
            {
                return BadRequest(new { message = "Job data cannot be null." });
            }

            try
            {
                // Add the job post to the JobPosts table
                _context.JobPosts.Add(jobPost);

                // Save changes to the SQL database asynchronously
                await _context.SaveChangesAsync();

                // Return a success response with the new JobPostId
                return Ok(new
                {
                    message = "Internship Posted Successfully!",
                    jobId = jobPost.JobPostId
                });
            }
            catch (Exception ex)
            {
                // Return a 500 status code if there is a server or database error
                return StatusCode(500, new
                {
                    message = "An error occurred while saving to the database.",
                    error = ex.Message
                });
            }
        }
    }
}