using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SkillPivotAPI.Models
{
    public class JobPost
    {
        [Key] // Primary Key for the JobPost table
        public int JobPostId { get; set; }

        [Required] // Ensures the Job Title is not empty
        public string JobTitle { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        public string TechnologyStack { get; set; } = string.Empty;

        public string JobType { get; set; } = string.Empty; // e.g., On-site, Remote, Hybrid

        public string JobRole { get; set; } = "Intern"; // Default role is Intern

        public string Status { get; set; } = "Active"; // e.g., Active, Closed

        public DateTime PostedDate { get; set; } = DateTime.Now;

        // Foreign Key for Company
        [Required]
        public int CompanyId { get; set; }

        // Navigation Property: Link to the Company model
        [ForeignKey("CompanyId")]
        [JsonIgnore] // Prevents object cycles when returning JSON data
        public virtual Company? Company { get; set; }
    }
}