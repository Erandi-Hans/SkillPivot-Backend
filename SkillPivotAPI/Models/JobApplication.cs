using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkillPivotAPI.Models
{
    /**
     * Entity model representing the 'JobApplications' table in the SQL database.
     * Stores relationship data between Students and Job Posts.
     */
    public class JobApplication
    {
        [Key] // Primary Key: Unique identifier for each job application record
        public int ApplicationId { get; set; }

        [Required] // Foreign Key Reference: Links to the specific job advertisement
        public int JobPostId { get; set; }

        [Required] // Foreign Key Reference: Identifies the student who is applying
        public int StudentId { get; set; }

        // Timestamp of when the application was successfully submitted
        [Required]
        public DateTime AppliedDate { get; set; } = DateTime.Now;

        // Life-cycle status of the application (Default: Pending)
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Pending";
    }
}