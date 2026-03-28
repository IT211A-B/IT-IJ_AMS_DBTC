using AMS_Backend.Models;
using AMS_Backend.DTO.StudentDTO;

namespace AMS_Backend.Services.ServiceStudent
{
    public interface IStudentService
    {
        Task<IEnumerable<ReadStudentDTO>> GetAllStudents();
        Task<ReadStudentDTO> GetStudentById(Guid id);
        Task<ReadStudentDTO> AddStudent(CreateStudentDTO dto);
        Task UpdateStudent(Guid id, UpdateStudentDTO dto);
        Task DeleteStudent(Guid id);
    }
}