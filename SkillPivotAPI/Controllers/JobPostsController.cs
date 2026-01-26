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

        // GET: api/JobPosts
        // Retrieve all job postings from the database
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobPost>>> GetJobPosts()
        {
            return await _context.JobPosts.ToListAsync();
        }

        // POST: api/JobPosts
        // Create a new job posting in the database
        [HttpPost]
        public async Task<ActionResult<JobPost>> PostJobPost(JobPost jobPost)
        {
            _context.JobPosts.Add(jobPost);
            await _context.SaveChangesAsync();

            // Returning the created job post details
            return Ok(jobPost);
        }
    }
}