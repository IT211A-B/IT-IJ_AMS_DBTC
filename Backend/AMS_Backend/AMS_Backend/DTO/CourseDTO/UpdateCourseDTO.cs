using System.ComponentModel.DataAnnotations;

namespace AMS_Backend.DTO.CourseDTO
{
    public class UpdateCourseDTO
    {
        [Required] public string Name { get; set; } = string.Empty;
        [Required] public string Description { get; set; } = string.Empty;
    }
}
