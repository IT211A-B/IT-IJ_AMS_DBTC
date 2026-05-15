using AMS_Backend.DTO.CourseDTO;

namespace AMS_Backend.Services.ServiceCourse
{
    public interface ICourseService
    {
        Task<IEnumerable<ReadCourseDTO>> GetAllCoursesAsync();
        Task<ReadCourseDTO?> GetCourseByIdAsync(Guid id);
        Task<IEnumerable<ReadCourseDTO>> GetCoursesByTeacherAsync(Guid teacherId);
        Task<ReadCourseDTO> CreateCourseAsync(CreateCourseDTO dto);
        Task<ReadCourseDTO?> UpdateCourseAsync(Guid id, UpdateCourseDTO dto);
        Task<bool> DeleteCourseAsync(Guid id);
        Task<bool> TeacherExistsAsync(Guid teacherId);
    }
}