using AMS_Backend.DTO.EnrollmentDTO;

namespace AMS_Backend.Services.ServiceEnrollment
{
    public interface IEnrollmentService
    {
        Task<IEnumerable<ReadEnrollmentDTO>> GetAllEnrollmentsAsync();
        Task<ReadEnrollmentDTO?> GetEnrollmentByIdAsync(Guid id);
        Task<IEnumerable<ReadEnrollmentDTO>> GetEnrollmentsByStudentAsync(Guid studentId);
        Task<IEnumerable<ReadEnrollmentDTO>> GetEnrollmentsByCourseAsync(Guid courseId);
        Task<ReadEnrollmentDTO> CreateEnrollmentAsync(CreateEnrollmentDTO dto);
        Task<ReadEnrollmentDTO?> UpdateEnrollmentStatusAsync(Guid id, UpdateEnrollmentStatusDTO dto);
        Task<bool> DeleteEnrollmentAsync(Guid id);
        Task<bool> IsAlreadyEnrolledAsync(Guid studentId, Guid courseId);
        Task<bool> StudentExistsAsync(Guid studentId);
        Task<bool> CourseExistsAsync(Guid courseId);
    }
}