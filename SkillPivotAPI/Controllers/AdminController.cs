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

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Get all Users (For User Management Page)
        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // 2. Get all Companies (For Verification Page)
        [HttpGet("companies")]
        public async Task<ActionResult<IEnumerable<Company>>> GetCompanies()
        {
            return await _context.Companies.ToListAsync();
        }

        // 3. Verify a Company (Admin approves a company)
        [HttpPut("verify-company/{id}")]
        public async Task<IActionResult> VerifyCompany(int id)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company == null) return NotFound();

            company.IsVerified = true;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Company verified successfully!" });
        }

        // 4. Get all Job Posts (For Moderation)
        [HttpGet("jobs")]
        public async Task<ActionResult<IEnumerable<JobPost>>> GetJobs()
        {
            return await _context.JobPosts.Include(j => j.Company).ToListAsync();
        }
    }
}