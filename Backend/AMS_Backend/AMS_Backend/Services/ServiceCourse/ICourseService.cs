using AMS_Backend.DTO.CourseDTO;
using AMS_Backend.Models;

namespace AMS_Backend.Services.ServiceCourse
{
    public interface ICourseService
    {
        Task<IEnumerable<ReadCourseDTO>> GetAllCourses();
        Task<ReadCourseDTO> GetCourseById(Guid id);
        Task<ReadCourseDTO> AddCourse(CreateCourseDTO courseDto);
        Task UpdateCourse(Guid id, UpdateCourseDTO courseDto);
        Task DeleteCourse(Guid id);
    }
}