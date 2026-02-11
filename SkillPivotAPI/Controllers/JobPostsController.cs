using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillPivotAPI.Data;
using SkillPivotAPI.Models;

namespace SkillPivotAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobPostsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public JobPostsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all job posts from the database including related company data.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobPost>>> GetJobPosts()
        {
            try
            {
                // Fetch jobs from database
                var jobs = await _context.JobPosts.ToListAsync();

                if (jobs == null || !jobs.Any())
                {
                    return Ok(new List<JobPost>()); // Return empty list instead of 404 to avoid frontend filter errors
                }

                return Ok(jobs);
            }
            catch (Exception ex)
            {
                // Return detailed error message for debugging
                return StatusCode(500, new { message = "Database retrieval error.", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostJob([FromBody] JobPost jobPost)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

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

        [HttpPut("{id}")]
        public async Task<IActionResult> PutJobPost(int id, [FromBody] JobPost jobPost)
        {
            if (id != jobPost.JobPostId) return BadRequest(new { message = "ID mismatch." });

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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJobPost(int id)
        {
            var jobPost = await _context.JobPosts.FindAsync(id);
            if (jobPost == null) return NotFound(new { message = "Job not found." });

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