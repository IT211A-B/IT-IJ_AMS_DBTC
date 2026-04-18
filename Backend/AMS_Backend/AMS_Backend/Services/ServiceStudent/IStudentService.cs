using AMS_Backend.DTO.StudentDTO;

namespace AMS_Backend.Services.ServiceStudent
{
    public interface IStudentService
    {
        Task<IEnumerable<ReadStudentDTO>> GetAllStudentsAsync();
        Task<ReadStudentDTO?> GetStudentByIdAsync(Guid id);
        Task<ReadStudentDTO> CreateStudentAsync(CreateStudentDTO dto);
        Task<ReadStudentDTO?> UpdateStudentAsync(Guid id, UpdateStudentDTO dto);
        Task<bool> DeleteStudentAsync(Guid id);
        Task<bool> StudentNumberExistsAsync(string studentNumber);
        Task<bool> EmailExistsAsync(string email, Guid? excludeId = null);
    }
}