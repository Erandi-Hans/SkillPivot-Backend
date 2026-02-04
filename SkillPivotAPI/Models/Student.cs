namespace SkillPivotAPI.Models
{
    public class Student
    {
        public int StudentId { get; set; }
        public int UserId { get; set; }

        // Academic Details
        public string University { get; set; } = string.Empty;
        public string Degree { get; set; } = string.Empty;
        public string GPA { get; set; } = string.Empty;

        // Professional Profile
        public string Skills { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;

        // Verification Details
        public bool IsVerified { get; set; } = false;
        public string NicDocumentPath { get; set; } = string.Empty;
    }
}