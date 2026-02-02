using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillPivotAPI.Data;
using SkillPivotAPI.Models;

namespace SkillPivotAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor to inject the database context.
        /// </summary>
        public CompaniesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a specific company's profile details by its unique ID.
        /// GET: api/Companies/5
        /// </summary>
        /// <param name="id">The unique identifier of the company.</param>
        /// <returns>Company object if found, otherwise 404 NotFound.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Company>> GetCompany(int id)
        {
            // Find the company in the database using the provided ID
            var company = await _context.Companies.FindAsync(id);

            // Return 404 if the company record does not exist
            if (company == null)
            {
                return NotFound(new { message = "Company profile not found." });
            }

            return Ok(company);
        }

        /// <summary>
        /// Updates an existing company profile. Supports multipart/form-data for image uploads.
        /// PUT: api/Companies/5
        /// </summary>
        /// <param name="id">The ID of the company to update.</param>
        /// <param name="updatedCompany">The updated profile data from the form.</param>
        /// <param name="Logo">Optional image file for the company logo.</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompany(int id, [FromForm] Company updatedCompany, IFormFile? Logo)
        {
            // Fetch the existing record from the database
            var company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                return NotFound(new { message = "Company not found." });
            }

            // Update general company information
            company.CompanyName = updatedCompany.CompanyName;
            company.Industry = updatedCompany.Industry;
            company.Description = updatedCompany.Description;
            company.Website = updatedCompany.Website;
            company.ContactEmail = updatedCompany.ContactEmail;
            company.Location = updatedCompany.Location;

            // Handle Logo Upload Logic:
            // 1. Check if a new logo file is provided (IFormFile Logo)
            // 2. Save the file to a physical directory (e.g., wwwroot/uploads/logos)
            // 3. Update the company.LogoUrl property with the relative path

            // Mark the entity state as modified and save changes to SQL Server
            _context.Entry(company).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyExists(id)) return NotFound();
                else throw;
            }

            return Ok(company);
        }

        /// <summary>
        /// Helper method to check if a company exists in the database.
        /// </summary>
        private bool CompanyExists(int id)
        {
            return _context.Companies.Any(e => e.CompanyId == id);
        }
    }
}