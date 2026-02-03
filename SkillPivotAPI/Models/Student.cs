using System.ComponentModel.DataAnnotations;

namespace SkillPivotAPI.Controllers
{
    /// <summary>
    /// Represents a student profile in the system, 
    /// containing academic and demographic information.
    /// </summary>
    public class Student
    {
        // Unique identifier for the Student record (Primary Key)
        [Key]
        public int StudentId { get; set; }

        // Reference to the main User account (Foreign Key)
        [Required]
        public int UserId { get; set; }

        // The name of the educational institution
        public string? University { get; set; }

        // The specific program or degree of study
        public string? Degree { get; set; }

        // Grade Point Average (GPA) of the student
        public string? GPA { get; set; }

        // List of professional and technical skills
        public string? Skills { get; set; }

        // The gender identity of the student for demographic purposes
        public string? Gender { get; set; }

        // File path or URL where the NIC/ID document is stored
        public string? NicDocumentPath { get; set; }

        // Indicates whether the student's profile has been verified by an admin
        public bool IsVerified { get; set; } = false;
    }
}