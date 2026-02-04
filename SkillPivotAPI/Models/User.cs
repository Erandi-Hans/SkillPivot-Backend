using System.ComponentModel.DataAnnotations;

namespace SkillPivotAPI.Models
{
    public class User
    {
        [Key] // Defines this as the Primary Key
        public int UserId { get; set; }

        [Required]
        [EmailAddress] // Validates email format
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty; // e.g., "Student" or "Company"

        [Required]
        public string Firstname { get; set; } = string.Empty;

        [Required]
        public string Lastname { get; set; } = string.Empty;

        // OTP and Verification fields
        public string? Verificationcode { get; set; }
        public DateTime? OTPExpiry { get; set; }
        public bool IsVerified { get; set; } = false;

        // Profile details (Optional but recommended based on your UI)
        public string? Location { get; set; }
        public string? Industry { get; set; }

        // ProfilePicture property to match the database column
        public string? ProfilePicture { get; set; }
    }
}