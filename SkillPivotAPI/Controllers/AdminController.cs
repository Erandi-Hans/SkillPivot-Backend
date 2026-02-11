using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillPivotAPI.Data;
using SkillPivotAPI.Models;

namespace SkillPivotAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor: Injects the Database Context to interact with the system's data.
        /// </summary>
        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// HTTP GET: Retrieves the complete list of all registered users (Students, Companies, etc.).
        /// Targeted for the Admin's User Management Dashboard.
        /// </summary>
        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        /// <summary>
        /// HTTP GET: Retrieves all registered company profiles.
        /// Primarily used on the Admin Verification page to review company registrations.
        /// </summary>
        [HttpGet("companies")]
        public async Task<ActionResult<IEnumerable<Company>>> GetCompanies()
        {
            return await _context.Companies.ToListAsync();
        }

        /// <summary>
        /// HTTP PUT: Grants "Verified" status to a specific company based on its Unique ID.
        /// This action validates the company's legitimacy within the platform.
        /// </summary>
        /// <param name="id">The unique identifier of the company to be verified.</param>
        [HttpPut("verify-company/{id}")]
        public async Task<IActionResult> VerifyCompany(int id)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                return NotFound(new { message = "Verification failed: Company record not found." });
            }

            company.IsVerified = true;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Company verified successfully!" });
        }

        /// <summary>
        /// HTTP GET: Retrieves all job posts currently available in the system.
        /// Includes related Company details for moderation and administrative oversight.
        /// </summary>
        [HttpGet("jobs")]
        public async Task<ActionResult<IEnumerable<JobPost>>> GetJobs()
        {
            // The .Include(j => j.Company) ensures company details are linked with each job post.
            return await _context.JobPosts.Include(j => j.Company).ToListAsync();
        }
    }
}