using System.ComponentModel.DataAnnotations;

namespace SkillPivotAPI.Models
{
    public class Company
    {
        [Key]
        public int CompanyId { get; set; }

        [Required]
        public string CompanyName { get; set; }

        // The official email address used for company-related communications
        public string ContactEmail { get; set; }

        // The specific field the company belongs to (e.g., Software Engineering, Networking)
        public string Industry { get; set; }

        // Optional: The company's official website URL for verification purposes
        public string? Website { get; set; }

        // Optional: A brief overview or history of the company
        public string? Description { get; set; }

      // The physical location/city where the company is based [cite: 9, 31]
        public string? Location { get; set; }

       // Status flag to indicate if the Admin has approved the company [cite: 26, 33]
        public bool IsVerified { get; set; } = false;

        // Automatically stores the date and time when the company registered
        public DateTime RegisteredDate { get; set; } = DateTime.Now;

       // Navigation property for a one-to-many relationship with JobPosts [cite: 26]
        public ICollection<JobPost>? JobPosts { get; set; }
    }
}