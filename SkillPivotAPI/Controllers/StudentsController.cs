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
        private readonly IWebHostEnvironment _environment;

        public StudentsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // 1. GET: Fetch student profile details by UserId
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<Student>> GetStudentByUserId(int userId)
        {
            var student = await _context.Students.AsNoTracking().FirstOrDefaultAsync(s => s.UserId == userId);
            if (student == null)
            {
                return NotFound(new { message = "Student profile not found." });
            }
            return Ok(student);
        }

        // 2. POST: Initialize a new student profile record
        [HttpPost]
        public async Task<ActionResult<Student>> PostStudent([FromBody] Student student)
        {
            if (student == null) return BadRequest();

            var existingStudent = await _context.Students.AnyAsync(s => s.UserId == student.UserId);
            if (existingStudent)
            {
                return Conflict(new { message = "A student profile already exists for this user." });
            }

            // Initialize non-nullable fields with empty strings to prevent database null constraint errors
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

        // 3. PUT: Update comprehensive student profile information
        [HttpPut("user/{userId}")]
        public async Task<IActionResult> UpdateStudentProfile(int userId, [FromBody] Student studentData)
        {
            if (studentData == null) return BadRequest();

            var existingStudent = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId);
            if (existingStudent == null)
            {
                return NotFound(new { message = "Student profile not found." });
            }

            // Map updated values to the existing entity
            existingStudent.University = studentData.University ?? "";
            existingStudent.Degree = studentData.Degree ?? "";
            existingStudent.GPA = studentData.GPA ?? "";
            existingStudent.Skills = studentData.Skills ?? "";
            existingStudent.Gender = studentData.Gender ?? "";

            // Keep existing path if a new one is not provided in the standard update
            existingStudent.NicDocumentPath = string.IsNullOrEmpty(studentData.NicDocumentPath)
                ? existingStudent.NicDocumentPath
                : studentData.NicDocumentPath;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "Student profile updated successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Database synchronization error", error = ex.Message });
            }
        }

        // 4. POST: Handle Identity Document Upload (NIC/Student ID)
        [HttpPost("upload-nic/{userId}")]
        public async Task<IActionResult> UploadNic(int userId, IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("No file uploaded.");

            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId);
            if (student == null) return NotFound("Student profile not found.");

            // Ensure the upload directory exists
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "nic");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            // Generate a unique filename to prevent overwriting
            var fileName = $"{userId}_NIC_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Save the relative path in the database for future retrieval
            student.NicDocumentPath = $"/uploads/nic/{fileName}";
            await _context.SaveChangesAsync();

            return Ok(new { message = "File uploaded successfully", path = student.NicDocumentPath });
        }
    }
}