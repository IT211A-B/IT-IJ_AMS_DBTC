using Microsoft.EntityFrameworkCore;
using AMS_Backend.Models;

namespace AMS_Backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }

        public DbSet<Teacher> Teachers { get; set; }

        public DbSet<Attendance> Attendances { get; set; }

        public DbSet<Course> Courses { get; set; }
    }
}