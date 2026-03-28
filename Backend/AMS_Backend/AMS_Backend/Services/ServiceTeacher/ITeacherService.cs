using AMS_Backend.DTO.TeacherDTO;
using AMS_Backend.Models;

namespace AMS_Backend.Services.ServiceTeacher
{
    public interface ITeacherService
    {
        Task<IEnumerable<ReadTeacherDTO>> GetAllTeachers();
        Task<ReadTeacherDTO> GetTeacherById(Guid id);
        Task<ReadTeacherDTO> AddTeacher(CreateTeacherDTO teacherDto);
        Task UpdateTeacher(Guid id, UpdateTeacherDTO teacherDto);
        Task DeleteTeacher(Guid id);
    }
}