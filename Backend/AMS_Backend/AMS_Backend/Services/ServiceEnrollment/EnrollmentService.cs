using AMS_Backend.Data;
using AMS_Backend.DTO.EnrollmentDTO;
using AMS_Backend.Models;
using AMS_Backend.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AMS_Backend.Services.ServiceEnrollment
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IGenericRepository<Enrollment> _repo;
        private readonly ApplicationDbContext _context;

        public EnrollmentService(IGenericRepository<Enrollment> repo, ApplicationDbContext context)
        {
            _repo = repo;
            _context = context;
        }

        public async Task<IEnumerable<ReadEnrollmentDTO>> GetAllEnrollmentsAsync()
        {
            var enrollments = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .AsNoTracking()
                .ToListAsync();

            return enrollments.Select(MapToReadDTO);
        }

        public async Task<ReadEnrollmentDTO?> GetEnrollmentByIdAsync(Guid id)
        {
            var enrollment = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);

            return enrollment is null ? null : MapToReadDTO(enrollment);
        }

        public async Task<IEnumerable<ReadEnrollmentDTO>> GetEnrollmentsByStudentAsync(Guid studentId)
        {
            var enrollments = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .Where(e => e.StudentId == studentId)
                .AsNoTracking()
                .ToListAsync();

            return enrollments.Select(MapToReadDTO);
        }

        public async Task<IEnumerable<ReadEnrollmentDTO>> GetEnrollmentsByCourseAsync(Guid courseId)
        {
            var enrollments = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .Where(e => e.CourseId == courseId)
                .AsNoTracking()
                .ToListAsync();

            return enrollments.Select(MapToReadDTO);
        }

        public async Task<ReadEnrollmentDTO> CreateEnrollmentAsync(CreateEnrollmentDTO dto)
        {
            var enrollment = new Enrollment
            {
                StudentId = dto.StudentId,
                CourseId = dto.CourseId,
                Status = "Active"
            };

            await _repo.CreateAsync(enrollment);

            var created = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .FirstAsync(e => e.Id == enrollment.Id);

            return MapToReadDTO(created);
        }

        public async Task<ReadEnrollmentDTO?> UpdateEnrollmentStatusAsync(Guid id, UpdateEnrollmentStatusDTO dto)
        {
            var enrollment = await _repo.GetByIdAsync(id);
            if (enrollment is null) return null;

            enrollment.Status = dto.Status;
            await _repo.UpdateAsync(enrollment);

            var updated = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .FirstAsync(e => e.Id == id);

            return MapToReadDTO(updated);
        }

        public async Task<bool> DeleteEnrollmentAsync(Guid id)
            => await _repo.DeleteAsync(id);

        public async Task<bool> IsAlreadyEnrolledAsync(Guid studentId, Guid courseId)
            => await _context.Enrollments.AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId);

        public async Task<bool> StudentExistsAsync(Guid studentId)
            => await _context.Students.AnyAsync(s => s.Id == studentId);

        public async Task<bool> CourseExistsAsync(Guid courseId)
            => await _context.Courses.AnyAsync(c => c.Id == courseId);

        private static ReadEnrollmentDTO MapToReadDTO(Enrollment e) => new()
        {
            Id = e.Id,
            StudentId = e.StudentId,
            StudentName = $"{e.Student.FirstName} {e.Student.LastName}",
            StudentNumber = e.Student.StudentNumber,
            CourseId = e.CourseId,
            CourseCode = e.Course.CourseCode,
            CourseName = e.Course.CourseName,
            Status = e.Status,
            EnrolledAt = e.EnrolledAt
        };
    }
}