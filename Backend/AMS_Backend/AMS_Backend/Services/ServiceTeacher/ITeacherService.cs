using AMS_Backend.DTO.TeacherDTO;
using AMS_Backend.Models;

namespace AMS_Backend.Services.ServiceTeacher
{
    public interface ITeacherService
    {
        Task<IEnumerable<ReadTeacherDTO>> GetAllTeachers();
        Task<ReadTeacherDTO> GetTeacherById(int id);
        Task<ReadTeacherDTO> AddTeacher(CreateTeacherDTO teacherDto);
        Task UpdateTeacher(int id, UpdateTeacherDTO teacherDto);
        Task DeleteTeacher(int id);
    }
}