namespace SkillPivotAPI.Models
{
    public class RegisterDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

        public string? CompanyName { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
    }
}