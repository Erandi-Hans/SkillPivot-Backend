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

        // New Profile Fields
        public string TargetRole { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string Experience { get; set; } = string.Empty;
        public string Languages { get; set; } = string.Empty;

        // These will store JSON strings from the React frontend
        public string Projects { get; set; } = "[]";
        public string Certifications { get; set; } = "[]";

        // Verification Details
        public bool IsVerified { get; set; } = false;
        public string NicDocumentPath { get; set; } = string.Empty;

        public string Medium { get; set; } = string.Empty;
    }
}