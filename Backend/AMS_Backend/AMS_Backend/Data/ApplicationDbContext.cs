using AMS_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace AMS_Backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Attendance> Attendances { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── Student ──────────────────────────────────────────────────────
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.HasIndex(s => s.StudentNumber).IsUnique();
                entity.HasIndex(s => s.Email).IsUnique();
                entity.Property(s => s.Id).ValueGeneratedOnAdd();
            });

            // ── Teacher ──────────────────────────────────────────────────────
            modelBuilder.Entity<Teacher>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.HasIndex(t => t.EmployeeNumber).IsUnique();
                entity.HasIndex(t => t.Email).IsUnique();
                entity.Property(t => t.Id).ValueGeneratedOnAdd();
            });

            // ── Course ───────────────────────────────────────────────────────
            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.HasIndex(c => c.CourseCode);
                entity.Property(c => c.Id).ValueGeneratedOnAdd();

                entity.HasOne(c => c.Teacher)
                      .WithMany(t => t.Courses)
                      .HasForeignKey(c => c.TeacherId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ── Enrollment ───────────────────────────────────────────────────
            modelBuilder.Entity<Enrollment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                // A student can only be enrolled once per course
                entity.HasIndex(e => new { e.StudentId, e.CourseId }).IsUnique();

                entity.HasOne(e => e.Student)
                      .WithMany(s => s.Enrollments)
                      .HasForeignKey(e => e.StudentId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Course)
                      .WithMany(c => c.Enrollments)
                      .HasForeignKey(e => e.CourseId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ── Attendance ───────────────────────────────────────────────────
            modelBuilder.Entity<Attendance>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Id).ValueGeneratedOnAdd();

                // A student can only have one attendance record per course per date
                entity.HasIndex(a => new { a.StudentId, a.CourseId, a.Date }).IsUnique();

                entity.HasOne(a => a.Student)
                      .WithMany(s => s.Attendances)
                      .HasForeignKey(a => a.StudentId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(a => a.Course)
                      .WithMany(c => c.Attendances)
                      .HasForeignKey(a => a.CourseId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}