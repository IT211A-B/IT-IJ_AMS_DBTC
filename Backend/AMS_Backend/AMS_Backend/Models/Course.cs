using System.ComponentModel.DataAnnotations;

namespace AMS_Backend.Models
{
    public class Course
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(20)]
        public string CourseCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string CourseName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public int Units { get; set; }

        [MaxLength(50)]
        public string? Schedule { get; set; }

        [MaxLength(50)]
        public string? Room { get; set; }

        [MaxLength(20)]
        public string? Semester { get; set; }

        [MaxLength(20)]
        public string? AcademicYear { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign key
        public Guid TeacherId { get; set; }

        // Navigation properties
        public Teacher Teacher { get; set; } = null!;
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    }
}