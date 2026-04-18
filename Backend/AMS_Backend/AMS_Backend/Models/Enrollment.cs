using System.ComponentModel.DataAnnotations;

namespace AMS_Backend.Models
{
    public class Enrollment
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

        [MaxLength(20)]
        public string Status { get; set; } = "Active"; // Active, Dropped, Completed

        // Foreign keys
        public Guid StudentId { get; set; }
        public Guid CourseId { get; set; }

        // Navigation properties
        public Student Student { get; set; } = null!;
        public Course Course { get; set; } = null!;
    }
}