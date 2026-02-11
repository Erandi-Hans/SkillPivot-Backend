using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillPivotAPI.Data;
using SkillPivotAPI.Models;

namespace SkillPivotAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobApplicationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // Dependency injection to initialize the database context
        public JobApplicationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Fetches all job applications with linked job posting details.
        /// This mimics a SQL JOIN by selecting fields from both Application and JobPost tables.
        /// </summary>
        /// <returns>A status code with the list of joined application data</returns>
        [HttpGet]
        public async Task<IActionResult> GetApplications()
        {
            try
            {
                // Projects the data from JobApplications and related JobPosts into a flat object structure
                var applications = await _context.JobApplications
                    .Select(app => new
                    {
                        app.ApplicationId,
                        app.JobPostId,
                        app.StudentId,
                        app.AppliedDate,
                        app.Status,
                        // Manual join: Retrieving the Job Title matching the application's JobPostId
                        JobTitle = _context.JobPosts
                                    .Where(j => j.JobPostId == app.JobPostId)
                                    .Select(j => j.JobTitle)
                                    .FirstOrDefault(),
                        // Manual join: Retrieving the Company Name matching the application's JobPostId
                        CompanyName = _context.JobPosts
                                    .Where(j => j.JobPostId == app.JobPostId)
                                    .Select(j => j.CompanyName)
                                    .FirstOrDefault()
                    })
                    .ToListAsync();

                return Ok(applications);
            }
            catch (Exception ex)
            {
                // Logs server-side errors and returns a 500 status code
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Processes a new job application submission from the student.
        /// </summary>
        /// <param name="application">Application details received from the frontend</param>
        /// <returns>Confirmation message or validation errors</returns>
        [HttpPost]
        public async Task<IActionResult> ApplyForJob([FromBody] JobApplication application)
        {
            // Validates that the input data meets the model's annotation requirements
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Stages the new application record for insertion
                _context.JobApplications.Add(application);

                // Executes the SQL INSERT command in the database
                await _context.SaveChangesAsync();

                return Ok(new { message = "Application submitted successfully!" });
            }
            catch (Exception ex)
            {
                // Returns 500 error if DB operation fails (e.g., integrity constraints)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}