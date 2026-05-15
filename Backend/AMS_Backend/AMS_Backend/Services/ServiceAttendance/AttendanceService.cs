using AMS_Backend.Data;
using AMS_Backend.DTO.AttendanceDTO;
using AMS_Backend.Models;
using AMS_Backend.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AMS_Backend.Services.ServiceAttendance
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IGenericRepository<Attendance> _repo;
        private readonly ApplicationDbContext _context;

        public AttendanceService(IGenericRepository<Attendance> repo, ApplicationDbContext context)
        {
            _repo = repo;
            _context = context;
        }

        public async Task<IEnumerable<ReadAttendanceDTO>> GetAllAttendancesAsync()
        {
            var records = await _context.Attendances
                .Include(a => a.Student)
                .Include(a => a.Course)
                .AsNoTracking()
                .OrderByDescending(a => a.Date)
                .ToListAsync();

            return records.Select(MapToReadDTO);
        }

        public async Task<ReadAttendanceDTO?> GetAttendanceByIdAsync(Guid id)
        {
            var record = await _context.Attendances
                .Include(a => a.Student)
                .Include(a => a.Course)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);

            return record is null ? null : MapToReadDTO(record);
        }

        public async Task<IEnumerable<ReadAttendanceDTO>> GetAttendanceByStudentAsync(Guid studentId)
        {
            var records = await _context.Attendances
                .Include(a => a.Student)
                .Include(a => a.Course)
                .Where(a => a.StudentId == studentId)
                .AsNoTracking()
                .OrderByDescending(a => a.Date)
                .ToListAsync();

            return records.Select(MapToReadDTO);
        }

        public async Task<IEnumerable<ReadAttendanceDTO>> GetAttendanceByCourseAsync(Guid courseId)
        {
            var records = await _context.Attendances
                .Include(a => a.Student)
                .Include(a => a.Course)
                .Where(a => a.CourseId == courseId)
                .AsNoTracking()
                .OrderByDescending(a => a.Date)
                .ThenBy(a => a.Student.LastName)
                .ToListAsync();

            return records.Select(MapToReadDTO);
        }

        public async Task<IEnumerable<ReadAttendanceDTO>> GetAttendanceByCourseAndDateAsync(Guid courseId, DateOnly date)
        {
            var records = await _context.Attendances
                .Include(a => a.Student)
                .Include(a => a.Course)
                .Where(a => a.CourseId == courseId && a.Date == date)
                .AsNoTracking()
                .OrderBy(a => a.Student.LastName)
                .ThenBy(a => a.Student.FirstName)
                .ToListAsync();

            return records.Select(MapToReadDTO);
        }

        public async Task<IEnumerable<ReadAttendanceDTO>> GetAttendanceByStudentAndCourseAsync(Guid studentId, Guid courseId)
        {
            var records = await _context.Attendances
                .Include(a => a.Student)
                .Include(a => a.Course)
                .Where(a => a.StudentId == studentId && a.CourseId == courseId)
                .AsNoTracking()
                .OrderByDescending(a => a.Date)
                .ToListAsync();

            return records.Select(MapToReadDTO);
        }

        public async Task<AttendanceSummaryDTO?> GetAttendanceSummaryAsync(Guid studentId, Guid courseId)
        {
            var student = await _context.Students.FindAsync(studentId);
            var course = await _context.Courses.FindAsync(courseId);

            if (student is null || course is null) return null;

            var records = await _context.Attendances
                .Where(a => a.StudentId == studentId && a.CourseId == courseId)
                .AsNoTracking()
                .ToListAsync();

            int total = records.Count;
            int present = records.Count(a => a.Status == "Present");
            int absent = records.Count(a => a.Status == "Absent");
            int late = records.Count(a => a.Status == "Late");
            int excused = records.Count(a => a.Status == "Excused");

            // Attendance percentage: Present + Late count as attended
            double percentage = total > 0
                ? Math.Round((double)(present + late) / total * 100, 2)
                : 0;

            return new AttendanceSummaryDTO
            {
                StudentId = studentId,
                StudentName = $"{student.FirstName} {student.LastName}",
                StudentNumber = student.StudentNumber,
                CourseId = courseId,
                CourseCode = course.CourseCode,
                CourseName = course.CourseName,
                TotalDays = total,
                Present = present,
                Absent = absent,
                Late = late,
                Excused = excused,
                AttendancePercentage = percentage
            };
        }

        public async Task<ReadAttendanceDTO> CreateAttendanceAsync(CreateAttendanceDTO dto)
        {
            var attendance = new Attendance
            {
                StudentId = dto.StudentId,
                CourseId = dto.CourseId,
                Date = dto.Date,
                Status = dto.Status,
                Remarks = dto.Remarks?.Trim()
            };

            await _repo.CreateAsync(attendance);

            var created = await _context.Attendances
                .Include(a => a.Student)
                .Include(a => a.Course)
                .FirstAsync(a => a.Id == attendance.Id);

            return MapToReadDTO(created);
        }

        public async Task<IEnumerable<ReadAttendanceDTO>> BulkCreateAttendanceAsync(BulkCreateAttendanceDTO dto)
        {
            var records = dto.Entries.Select(entry => new Attendance
            {
                StudentId = entry.StudentId,
                CourseId = dto.CourseId,
                Date = dto.Date,
                Status = entry.Status,
                Remarks = entry.Remarks?.Trim()
            }).ToList();

            await _context.Attendances.AddRangeAsync(records);
            await _context.SaveChangesAsync();

            var ids = records.Select(r => r.Id).ToList();
            var created = await _context.Attendances
                .Include(a => a.Student)
                .Include(a => a.Course)
                .Where(a => ids.Contains(a.Id))
                .OrderBy(a => a.Student.LastName)
                .AsNoTracking()
                .ToListAsync();

            return created.Select(MapToReadDTO);
        }

        public async Task<ReadAttendanceDTO?> UpdateAttendanceAsync(Guid id, UpdateAttendanceDTO dto)
        {
            var record = await _repo.GetByIdAsync(id);
            if (record is null) return null;

            record.Status = dto.Status;
            record.Remarks = dto.Remarks?.Trim();

            await _repo.UpdateAsync(record);

            var updated = await _context.Attendances
                .Include(a => a.Student)
                .Include(a => a.Course)
                .FirstAsync(a => a.Id == id);

            return MapToReadDTO(updated);
        }

        public async Task<bool> DeleteAttendanceAsync(Guid id)
            => await _repo.DeleteAsync(id);

        public async Task<bool> AttendanceRecordExistsAsync(Guid studentId, Guid courseId, DateOnly date, Guid? excludeId = null)
        {
            var query = _context.Attendances
                .Where(a => a.StudentId == studentId && a.CourseId == courseId && a.Date == date);

            if (excludeId.HasValue)
                query = query.Where(a => a.Id != excludeId.Value);

            return await query.AnyAsync();
        }

        public async Task<bool> StudentIsEnrolledAsync(Guid studentId, Guid courseId)
            => await _context.Enrollments.AnyAsync(e =>
                e.StudentId == studentId &&
                e.CourseId == courseId &&
                e.Status == "Active");

        public async Task<bool> StudentExistsAsync(Guid studentId)
            => await _context.Students.AnyAsync(s => s.Id == studentId);

        public async Task<bool> CourseExistsAsync(Guid courseId)
            => await _context.Courses.AnyAsync(c => c.Id == courseId);

        private static ReadAttendanceDTO MapToReadDTO(Attendance a) => new()
        {
            Id = a.Id,
            StudentId = a.StudentId,
            StudentName = $"{a.Student.FirstName} {a.Student.LastName}",
            StudentNumber = a.Student.StudentNumber,
            CourseId = a.CourseId,
            CourseCode = a.Course.CourseCode,
            CourseName = a.Course.CourseName,
            Date = a.Date,
            Status = a.Status,
            Remarks = a.Remarks,
            RecordedAt = a.RecordedAt
        };
    }
}