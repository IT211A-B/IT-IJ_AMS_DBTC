using AMS_Backend.DTO.AttendanceDTO;

namespace AMS_Backend.Services.ServiceAttendance
{
    public interface IAttendanceService
    {
        Task<IEnumerable<ReadAttendanceDTO>> GetAllAttendancesAsync();
        Task<ReadAttendanceDTO?> GetAttendanceByIdAsync(Guid id);
        Task<IEnumerable<ReadAttendanceDTO>> GetAttendanceByStudentAsync(Guid studentId);
        Task<IEnumerable<ReadAttendanceDTO>> GetAttendanceByCourseAsync(Guid courseId);
        Task<IEnumerable<ReadAttendanceDTO>> GetAttendanceByCourseAndDateAsync(Guid courseId, DateOnly date);
        Task<IEnumerable<ReadAttendanceDTO>> GetAttendanceByStudentAndCourseAsync(Guid studentId, Guid courseId);
        Task<AttendanceSummaryDTO?> GetAttendanceSummaryAsync(Guid studentId, Guid courseId);
        Task<ReadAttendanceDTO> CreateAttendanceAsync(CreateAttendanceDTO dto);
        Task<IEnumerable<ReadAttendanceDTO>> BulkCreateAttendanceAsync(BulkCreateAttendanceDTO dto);
        Task<ReadAttendanceDTO?> UpdateAttendanceAsync(Guid id, UpdateAttendanceDTO dto);
        Task<bool> DeleteAttendanceAsync(Guid id);
        Task<bool> AttendanceRecordExistsAsync(Guid studentId, Guid courseId, DateOnly date, Guid? excludeId = null);
        Task<bool> StudentIsEnrolledAsync(Guid studentId, Guid courseId);
        Task<bool> StudentExistsAsync(Guid studentId);
        Task<bool> CourseExistsAsync(Guid courseId);
    }
}