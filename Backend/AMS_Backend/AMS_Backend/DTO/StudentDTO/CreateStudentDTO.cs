using System.ComponentModel.DataAnnotations;

namespace AMS_Backend.DTO.StudentDTO
{
    public class CreateStudentDTO
    {
        [Required] public string FirstName { get; set; } = string.Empty;
        [Required] public string LastName { get; set; } = string.Empty;
        [Required] public string Email { get; set; } = string.Empty;
    }
}
