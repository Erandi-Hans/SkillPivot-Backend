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

        public StudentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. GET: Student profile by UserId
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<Student>> GetStudentByUserId(int userId)
        {
            // .AsNoTracking() පාවිච්චි කිරීමෙන් performance වැඩි වෙනවා
            var student = await _context.Students.AsNoTracking().FirstOrDefaultAsync(s => s.UserId == userId);

            if (student == null)
            {
                return NotFound(new { message = "Student profile not found." });
            }
            return Ok(student);
        }

        // 2. POST: Create a new student profile
        [HttpPost]
        public async Task<ActionResult<Student>> PostStudent([FromBody] Student student)
        {
            if (student == null) return BadRequest();

            var existingStudent = await _context.Students.AnyAsync(s => s.UserId == student.UserId);
            if (existingStudent)
            {
                return Conflict(new { message = "A student profile already exists for this user." });
            }

            // අගයන් null නම් හිස් string එකක් දාන්න (Manual SQL table එකේ null allow කරලා නැති නිසා)
            student.University ??= "";
            student.Degree ??= "";
            student.GPA ??= "";
            student.Skills ??= "";
            student.Gender ??= "";
            student.NicDocumentPath ??= "";

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStudentByUserId), new { userId = student.UserId }, student);
        }

        // 3. PUT: Update student profile
        [HttpPut("user/{userId}")]
        public async Task<IActionResult> UpdateStudentProfile(int userId, [FromBody] Student studentData)
        {
            if (studentData == null) return BadRequest();

            var existingStudent = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId);

            if (existingStudent == null)
            {
                return NotFound(new { message = "Student profile not found." });
            }

            // දත්ත Update කිරීම
            existingStudent.University = studentData.University ?? "";
            existingStudent.Degree = studentData.Degree ?? "";
            existingStudent.GPA = studentData.GPA ?? "";
            existingStudent.Skills = studentData.Skills ?? "";
            existingStudent.Gender = studentData.Gender ?? "";
            existingStudent.NicDocumentPath = studentData.NicDocumentPath ?? existingStudent.NicDocumentPath;
            existingStudent.IsVerified = studentData.IsVerified;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "Profile updated successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating database", error = ex.Message });
            }
        }
    }
}