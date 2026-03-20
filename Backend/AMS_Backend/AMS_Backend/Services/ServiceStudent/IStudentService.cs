using AMS_Backend.Models;
using AMS_Backend.DTO.StudentDTO;

namespace AMS_Backend.Services.ServiceStudent
{
    public interface IStudentService
    {
        Task<IEnumerable<ReadStudentDTO>> GetAllStudents();
        Task<ReadStudentDTO> GetStudentById(int id);
        Task<ReadStudentDTO> AddStudent(CreateStudentDTO dto);
        Task UpdateStudent(int id, UpdateStudentDTO dto);
        Task DeleteStudent(int id);
    }
}