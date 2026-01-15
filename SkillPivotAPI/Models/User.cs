namespace SkillPivotAPI.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

        public string Firstname { get; set; } = string.Empty;
        public string Lastname { get; set; } = string.Empty;

        public string? Verificationcode { get; set; }
        public DateTime? OTPExpiry { get; set; }
        public bool IsVerified { get; set; } = false;
    }
}