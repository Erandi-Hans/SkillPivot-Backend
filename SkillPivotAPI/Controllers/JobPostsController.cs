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
        /// Retrieves all job posts from the database.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobPost>>> GetJobPosts()
        {
            try
            {
                var jobs = await _context.JobPosts.ToListAsync();
                return Ok(jobs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving data.", error = ex.Message });
            }
        }

        /// <summary>
        /// Creates a new job post in the database.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> PostJob([FromBody] JobPost jobPost)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (jobPost == null) return BadRequest(new { message = "Job data cannot be null." });

            try
            {
                _context.JobPosts.Add(jobPost);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Internship Posted Successfully!", jobId = jobPost.JobPostId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error saving to database.", error = ex.Message });
            }
        }

        // --- අලුතින් එකතු කළ කොටස් ---

        /// <summary>
        /// Updates an existing job post.
        /// Route: PUT /api/JobPosts/{id}
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutJobPost(int id, [FromBody] JobPost jobPost)
        {
            if (id != jobPost.JobPostId)
            {
                return BadRequest(new { message = "ID mismatch." });
            }

            _context.Entry(jobPost).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "Updated successfully!" });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.JobPosts.Any(e => e.JobPostId == id)) return NotFound();
                else throw;
            }
        }

        /// <summary>
        /// Deletes a job post from the database.
        /// Route: DELETE /api/JobPosts/{id}
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJobPost(int id)
        {
            var jobPost = await _context.JobPosts.FindAsync(id);
            if (jobPost == null)
            {
                return NotFound(new { message = "Job not found." });
            }

            try
            {
                _context.JobPosts.Remove(jobPost);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Deleted successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting record.", error = ex.Message });
            }
        }
    }
}