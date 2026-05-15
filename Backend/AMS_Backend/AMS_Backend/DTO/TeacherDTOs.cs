using System.ComponentModel.DataAnnotations;

namespace AMS_Backend.DTO.TeacherDTO
{
    public class CreateTeacherDTO
    {
        [Required(ErrorMessage = "First name is required.")]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required.")]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Employee number is required.")]
        [MaxLength(50)]
        public string EmployeeNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? ContactNumber { get; set; }

        [MaxLength(100)]
        public string? Department { get; set; }
    }

    public class ReadTeacherDTO
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public string EmployeeNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? ContactNumber { get; set; }
        public string? Department { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UpdateTeacherDTO
    {
        [Required(ErrorMessage = "First name is required.")]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required.")]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? ContactNumber { get; set; }

        [MaxLength(100)]
        public string? Department { get; set; }
    }
}