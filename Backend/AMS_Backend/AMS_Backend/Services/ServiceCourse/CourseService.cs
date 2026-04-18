using AMS_Backend.Data;
using AMS_Backend.DTO.CourseDTO;
using AMS_Backend.Models;
using AMS_Backend.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AMS_Backend.Services.ServiceCourse
{
    public class CourseService : ICourseService
    {
        private readonly IGenericRepository<Course> _repo;
        private readonly ApplicationDbContext _context;

        public CourseService(IGenericRepository<Course> repo, ApplicationDbContext context)
        {
            _repo = repo;
            _context = context;
        }

        public async Task<IEnumerable<ReadCourseDTO>> GetAllCoursesAsync()
        {
            var courses = await _context.Courses
                .Include(c => c.Teacher)
                .Include(c => c.Enrollments)
                .AsNoTracking()
                .ToListAsync();

            return courses.Select(MapToReadDTO);
        }

        public async Task<ReadCourseDTO?> GetCourseByIdAsync(Guid id)
        {
            var course = await _context.Courses
                .Include(c => c.Teacher)
                .Include(c => c.Enrollments)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            return course is null ? null : MapToReadDTO(course);
        }

        public async Task<IEnumerable<ReadCourseDTO>> GetCoursesByTeacherAsync(Guid teacherId)
        {
            var courses = await _context.Courses
                .Include(c => c.Teacher)
                .Include(c => c.Enrollments)
                .Where(c => c.TeacherId == teacherId)
                .AsNoTracking()
                .ToListAsync();

            return courses.Select(MapToReadDTO);
        }

        public async Task<ReadCourseDTO> CreateCourseAsync(CreateCourseDTO dto)
        {
            var course = new Course
            {
                CourseCode = dto.CourseCode.Trim().ToUpper(),
                CourseName = dto.CourseName.Trim(),
                Description = dto.Description?.Trim(),
                Units = dto.Units,
                Schedule = dto.Schedule?.Trim(),
                Room = dto.Room?.Trim(),
                Semester = dto.Semester?.Trim(),
                AcademicYear = dto.AcademicYear?.Trim(),
                TeacherId = dto.TeacherId
            };

            await _repo.CreateAsync(course);

            // Reload with navigation properties
            var created = await _context.Courses
                .Include(c => c.Teacher)
                .Include(c => c.Enrollments)
                .FirstAsync(c => c.Id == course.Id);

            return MapToReadDTO(created);
        }

        public async Task<ReadCourseDTO?> UpdateCourseAsync(Guid id, UpdateCourseDTO dto)
        {
            var course = await _repo.GetByIdAsync(id);
            if (course is null) return null;

            course.CourseCode = dto.CourseCode.Trim().ToUpper();
            course.CourseName = dto.CourseName.Trim();
            course.Description = dto.Description?.Trim();
            course.Units = dto.Units;
            course.Schedule = dto.Schedule?.Trim();
            course.Room = dto.Room?.Trim();
            course.Semester = dto.Semester?.Trim();
            course.AcademicYear = dto.AcademicYear?.Trim();
            course.TeacherId = dto.TeacherId;

            await _repo.UpdateAsync(course);

            var updated = await _context.Courses
                .Include(c => c.Teacher)
                .Include(c => c.Enrollments)
                .FirstAsync(c => c.Id == id);

            return MapToReadDTO(updated);
        }

        public async Task<bool> DeleteCourseAsync(Guid id)
            => await _repo.DeleteAsync(id);

        public async Task<bool> TeacherExistsAsync(Guid teacherId)
            => await _context.Teachers.AnyAsync(t => t.Id == teacherId);

        private static ReadCourseDTO MapToReadDTO(Course c) => new()
        {
            Id = c.Id,
            CourseCode = c.CourseCode,
            CourseName = c.CourseName,
            Description = c.Description,
            Units = c.Units,
            Schedule = c.Schedule,
            Room = c.Room,
            Semester = c.Semester,
            AcademicYear = c.AcademicYear,
            TeacherId = c.TeacherId,
            TeacherName = $"{c.Teacher.FirstName} {c.Teacher.LastName}",
            EnrolledStudents = c.Enrollments.Count(e => e.Status == "Active"),
            CreatedAt = c.CreatedAt
        };
    }
}