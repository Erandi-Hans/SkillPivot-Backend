using Microsoft.EntityFrameworkCore;
using SkillPivotAPI.Models;

namespace SkillPivotAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<JobPost> JobPosts { get; set; }

        
        public DbSet<Student> Students { get; set; }

        public DbSet<JobApplication> JobApplications { get; set; }
    }
}