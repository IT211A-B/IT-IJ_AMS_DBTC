using System.ComponentModel.DataAnnotations;

namespace AMS_Backend.DTO.AttendanceDTO
{
    public class CreateAttendanceDTO
    {
        [Required(ErrorMessage = "Student ID is required.")]
        public Guid StudentId { get; set; }

        [Required(ErrorMessage = "Course ID is required.")]
        public Guid CourseId { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        public DateOnly Date { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [RegularExpression("Present|Absent|Late|Excused",
            ErrorMessage = "Status must be Present, Absent, Late, or Excused.")]
        public string Status { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Remarks { get; set; }
    }

    public class BulkCreateAttendanceDTO
    {
        [Required(ErrorMessage = "Course ID is required.")]
        public Guid CourseId { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        public DateOnly Date { get; set; }

        [Required(ErrorMessage = "At least one attendance record is required.")]
        [MinLength(1)]
        public List<AttendanceEntryDTO> Entries { get; set; } = new();
    }

    public class AttendanceEntryDTO
    {
        [Required(ErrorMessage = "Student ID is required.")]
        public Guid StudentId { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [RegularExpression("Present|Absent|Late|Excused",
            ErrorMessage = "Status must be Present, Absent, Late, or Excused.")]
        public string Status { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Remarks { get; set; }
    }

    public class ReadAttendanceDTO
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentNumber { get; set; } = string.Empty;
        public Guid CourseId { get; set; }
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Remarks { get; set; }
        public DateTime RecordedAt { get; set; }
    }

    public class UpdateAttendanceDTO
    {
        [Required(ErrorMessage = "Status is required.")]
        [RegularExpression("Present|Absent|Late|Excused",
            ErrorMessage = "Status must be Present, Absent, Late, or Excused.")]
        public string Status { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Remarks { get; set; }
    }

    public class AttendanceSummaryDTO
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentNumber { get; set; } = string.Empty;
        public Guid CourseId { get; set; }
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public int TotalDays { get; set; }
        public int Present { get; set; }
        public int Absent { get; set; }
        public int Late { get; set; }
        public int Excused { get; set; }
        public double AttendancePercentage { get; set; }
    }
}