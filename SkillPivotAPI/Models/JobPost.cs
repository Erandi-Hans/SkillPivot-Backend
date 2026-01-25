using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkillPivotAPI.Models
{
    public class JobPost
    {
        [Key]
        public int JobPostId { get; set; }

        [Required]
        // The title of the internship position (e.g., MERN Stack Intern, QA Intern)
        public string JobTitle { get; set; }

        [Required]
        // Detailed description of the job roles, responsibilities, and requirements
        public string Description { get; set; }

        // The specific technical skills required (e.g., .NET, React, Python) 
        public string? TechnologyStack { get; set; }

        // The type of work arrangement (e.g., Full-time, Part-time, Remote)
        public string? JobType { get; set; }

        // The specific role category (e.g., Backend Developer, UI/UX Designer)
        public string? JobRole { get; set; }

        // Current status of the post (e.g., Pending, Approved, Flagged) 
        public string Status { get; set; } = "Pending";

        // The date and time when the internship was posted
        public DateTime PostedDate { get; set; } = DateTime.Now;

        // Foreign Key to establish a relationship with the Company table 
        [ForeignKey("Company")]
        public int CompanyId { get; set; }

        // Navigation property to access Company details associated with this job post
        public Company? Company { get; set; }
    }
}