using System.ComponentModel.DataAnnotations;

namespace AMS_Backend.DTO.EnrollmentDTO
{
    public class CreateEnrollmentDTO
    {
        [Required(ErrorMessage = "Student ID is required.")]
        public Guid StudentId { get; set; }

        [Required(ErrorMessage = "Course ID is required.")]
        public Guid CourseId { get; set; }
    }

    public class ReadEnrollmentDTO
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentNumber { get; set; } = string.Empty;
        public Guid CourseId { get; set; }
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime EnrolledAt { get; set; }
    }

    public class UpdateEnrollmentStatusDTO
    {
        [Required(ErrorMessage = "Status is required.")]
        [RegularExpression("Active|Dropped|Completed",
            ErrorMessage = "Status must be Active, Dropped, or Completed.")]
        public string Status { get; set; } = string.Empty;
    }
}