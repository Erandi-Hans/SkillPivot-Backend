using Microsoft.EntityFrameworkCore;
using SkillPivotAPI.Models; 

namespace SkillPivotAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        
        public DbSet<User> Users { get; set; }
    }
}