using AMS_Backend.DTO.TeacherDTO;

namespace AMS_Backend.Services.ServiceTeacher
{
    public interface ITeacherService
    {
        Task<IEnumerable<ReadTeacherDTO>> GetAllTeachersAsync();
        Task<ReadTeacherDTO?> GetTeacherByIdAsync(Guid id);
        Task<ReadTeacherDTO> CreateTeacherAsync(CreateTeacherDTO dto);
        Task<ReadTeacherDTO?> UpdateTeacherAsync(Guid id, UpdateTeacherDTO dto);
        Task<bool> DeleteTeacherAsync(Guid id);
        Task<bool> EmployeeNumberExistsAsync(string employeeNumber);
        Task<bool> EmailExistsAsync(string email, Guid? excludeId = null);
    }
}