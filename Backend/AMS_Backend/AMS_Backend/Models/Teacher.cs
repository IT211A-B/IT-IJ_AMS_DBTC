using System.ComponentModel.DataAnnotations;

namespace AMS_Backend.Models
{
    public class Teacher
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string EmployeeNumber { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? ContactNumber { get; set; }

        [MaxLength(100)]
        public string? Department { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}