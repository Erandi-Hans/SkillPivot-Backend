using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillPivotAPI.Data;
using SkillPivotAPI.Models;

namespace SkillPivotAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor to inject the Database Context.
        /// </summary>
        public StudentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a complete student profile based on the provided UserId.
        /// Includes academic data, demographic info, and verification status.
        /// </summary>
        /// <param name="userId">The unique ID of the user.</param>
        /// <returns>Student object or NotFound if not exists.</returns>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<Student>> GetStudentByUserId(int userId)
        {
            // Fetching the student record linked to the specific UserId
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId);

            if (student == null)
            {
                return NotFound(new { message = "Student profile not found." });
            }

            return Ok(student);
        }

        /// <summary>
        /// Updates the student profile with new information from the client.
        /// Handles Academic, Demographic, and Verification document paths.
        /// </summary>
        /// <param name="userId">The unique ID of the user to update.</param>
        /// <param name="studentData">The updated student data from the request body.</param>
        [HttpPut("user/{userId}")]
        public async Task<IActionResult> UpdateStudentProfile(int userId, [FromBody] Student studentData)
        {
            // Checking if the student record exists in the database
            var existingStudent = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId);

            if (existingStudent == null)
            {
                return NotFound(new { message = "Student profile not found." });
            }

            // 1. Update Academic Information
            existingStudent.University = studentData.University;
            existingStudent.Degree = studentData.Degree;
            existingStudent.GPA = studentData.GPA;
            existingStudent.Skills = studentData.Skills;

            // 2. Update Personal Demographic Information
            existingStudent.Gender = studentData.Gender;

            // 3. Update Verification and Documents
            // Note: NicDocumentPath will store the file path after a successful upload
            existingStudent.NicDocumentPath = studentData.NicDocumentPath;
            
            // Note: Verification status is typically updated by an Admin, 
            // but we allow updating it if needed here.
            existingStudent.IsVerified = studentData.IsVerified;

            try
            {
                // Committing changes to the SQL Server database
                await _context.SaveChangesAsync();
                return Ok(new { message = "Full profile updated successfully!" });
            }
            catch (Exception ex)
            {
                // Catching database errors such as connection issues or constraint violations
                return StatusCode(500, new { message = "Database update failed: " + ex.Message });
            }
        }
    }
}