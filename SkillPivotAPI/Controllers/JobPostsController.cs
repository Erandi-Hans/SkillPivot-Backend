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

        // Constructor to initialize the database context
        public JobPostsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/JobPosts
        // Retrieves a list of all internship/job postings from the database
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobPost>>> GetJobPosts()
        {
            // Fetching all job post records as a list
            return await _context.JobPosts.ToListAsync();
        }

        // GET: api/JobPosts/5
        // Retrieves a specific job post by its unique ID
        // This is required for the CreatedAtAction response in the POST method
        [HttpGet("{id}")]
        public async Task<ActionResult<JobPost>> GetJobPost(int id)
        {
            var jobPost = await _context.JobPosts.FindAsync(id);

            // If the job post does not exist, return a 404 Not Found response
            if (jobPost == null)
            {
                return NotFound();
            }

            return jobPost;
        }

        // POST: api/JobPosts
        // Creates a new internship/job posting in the database
        [HttpPost]
        public async Task<ActionResult<JobPost>> PostJobPost(JobPost jobPost)
        {
            try
            {
                // Add the new job post object to the context
                _context.JobPosts.Add(jobPost);

                // Save changes to the SQL database asynchronously
                await _context.SaveChangesAsync();

                // Returns a 201 Created status code. 
                // It also provides the URL for the newly created resource via GetJobPost
                return CreatedAtAction(nameof(GetJobPost), new { id = jobPost.JobPostId }, jobPost);
            }
            catch (DbUpdateException)
            {
                // Handles database update errors, such as invalid Foreign Keys (e.g., wrong CompanyId)
                return BadRequest("Error saving data. Please ensure the CompanyId exists in the database.");
            }
        }
    }
}