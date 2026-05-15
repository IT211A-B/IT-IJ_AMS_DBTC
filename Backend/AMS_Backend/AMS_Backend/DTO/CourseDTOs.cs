using System.ComponentModel.DataAnnotations;

namespace AMS_Backend.DTO.CourseDTO
{
    public class CreateCourseDTO
    {
        [Required(ErrorMessage = "Course code is required.")]
        [MaxLength(20)]
        public string CourseCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Course name is required.")]
        [MaxLength(200)]
        public string CourseName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Range(0, 10, ErrorMessage = "Units must be between 0 and 10.")]
        public int Units { get; set; }

        [MaxLength(50)]
        public string? Schedule { get; set; }

        [MaxLength(50)]
        public string? Room { get; set; }

        [MaxLength(20)]
        public string? Semester { get; set; }

        [MaxLength(20)]
        public string? AcademicYear { get; set; }

        [Required(ErrorMessage = "Teacher ID is required.")]
        public Guid TeacherId { get; set; }
    }

    public class ReadCourseDTO
    {
        public Guid Id { get; set; }
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Units { get; set; }
        public string? Schedule { get; set; }
        public string? Room { get; set; }
        public string? Semester { get; set; }
        public string? AcademicYear { get; set; }
        public Guid TeacherId { get; set; }
        public string TeacherName { get; set; } = string.Empty;
        public int EnrolledStudents { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UpdateCourseDTO
    {
        [Required(ErrorMessage = "Course code is required.")]
        [MaxLength(20)]
        public string CourseCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Course name is required.")]
        [MaxLength(200)]
        public string CourseName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Range(0, 10, ErrorMessage = "Units must be between 0 and 10.")]
        public int Units { get; set; }

        [MaxLength(50)]
        public string? Schedule { get; set; }

        [MaxLength(50)]
        public string? Room { get; set; }

        [MaxLength(20)]
        public string? Semester { get; set; }

        [MaxLength(20)]
        public string? AcademicYear { get; set; }

        [Required(ErrorMessage = "Teacher ID is required.")]
        public Guid TeacherId { get; set; }
    }
}