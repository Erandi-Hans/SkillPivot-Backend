using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkillPivotAPI.Models
{
    public class JobPost
    {
        public int JobPostId { get; set; }
        public string JobTitle { get; set; }
        public string Description { get; set; }
        public string TechnologyStack { get; set; }
        public string JobType { get; set; } // On-site, Remote, etc.
        public string JobRole { get; set; }
        public string Status { get; set; }
        public DateTime PostedDate { get; set; }
        public int CompanyId { get; set; } 
    }
}