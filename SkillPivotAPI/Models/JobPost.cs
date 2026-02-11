using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SkillPivotAPI.Models
{
    public class JobPost
    {
        [Key]
        public int JobPostId { get; set; }

        [Required]
        public string JobTitle { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        // Matches 'stack' in frontend logic for filtering
        public string TechnologyStack { get; set; } = string.Empty;

        public string JobType { get; set; } = string.Empty;

        public string JobRole { get; set; } = "Intern";

        public string Status { get; set; } = "Active";

        public DateTime PostedDate { get; set; } = DateTime.Now;

        // Properties used for filtering in React frontend. 
        // Defined as nullable to handle potential NULL values in the database safely.
        public string? Category { get; set; } = string.Empty;
        public string? SubCategory { get; set; } = string.Empty;
        public string? CompanyName { get; set; } = string.Empty;
        public string? Location { get; set; } = string.Empty;

        [Required]
        public int CompanyId { get; set; }

        [ForeignKey("CompanyId")]
        // Critical: Prevents infinite loops/circular references during JSON serialization
        [JsonIgnore]
        public virtual Company? Company { get; set; }
    }
}