using AMS_Backend.DTO.CourseDTO;
using AMS_Backend.Models;

namespace AMS_Backend.Services.ServiceCourse
{
    public interface ICourseService
    {
        Task<IEnumerable<ReadCourseDTO>> GetAllCourses();
        Task<ReadCourseDTO> GetCourseById(int id);
        Task<ReadCourseDTO> AddCourse(CreateCourseDTO courseDto);
        Task UpdateCourse(int id, UpdateCourseDTO courseDto);
        Task DeleteCourse(int id);
    }
}