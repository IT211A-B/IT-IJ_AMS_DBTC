using AMS_Backend.Data;
using AMS_Backend.DTO.StudentDTO;
using AMS_Backend.Models;
using AMS_Backend.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AMS_Backend.Services.ServiceStudent
{
    public class StudentService : IStudentService
    {
        private readonly IGenericRepository<Student> _repo;
        private readonly ApplicationDbContext _context;

        public StudentService(IGenericRepository<Student> repo, ApplicationDbContext context)
        {
            _repo = repo;
            _context = context;
        }

        public async Task<IEnumerable<ReadStudentDTO>> GetAllStudentsAsync()
        {
            var students = await _repo.GetAllAsync();
            return students.Select(MapToReadDTO);
        }

        public async Task<ReadStudentDTO?> GetStudentByIdAsync(Guid id)
        {
            var student = await _repo.GetByIdAsync(id);
            return student is null ? null : MapToReadDTO(student);
        }

        public async Task<ReadStudentDTO> CreateStudentAsync(CreateStudentDTO dto)
        {
            var student = new Student
            {
                FirstName = dto.FirstName.Trim(),
                LastName = dto.LastName.Trim(),
                StudentNumber = dto.StudentNumber.Trim(),
                Email = dto.Email.Trim().ToLower(),
                ContactNumber = dto.ContactNumber?.Trim(),
                YearLevel = dto.YearLevel?.Trim(),
                Program = dto.Program?.Trim()
            };
            var created = await _repo.CreateAsync(student);
            return MapToReadDTO(created);
        }

        public async Task<ReadStudentDTO?> UpdateStudentAsync(Guid id, UpdateStudentDTO dto)
        {
            var student = await _repo.GetByIdAsync(id);
            if (student is null) return null;

            student.FirstName = dto.FirstName.Trim();
            student.LastName = dto.LastName.Trim();
            student.Email = dto.Email.Trim().ToLower();
            student.ContactNumber = dto.ContactNumber?.Trim();
            student.YearLevel = dto.YearLevel?.Trim();
            student.Program = dto.Program?.Trim();

            var updated = await _repo.UpdateAsync(student);
            return MapToReadDTO(updated);
        }

        public async Task<bool> DeleteStudentAsync(Guid id)
            => await _repo.DeleteAsync(id);

        public async Task<bool> StudentNumberExistsAsync(string studentNumber)
            => await _context.Students.AnyAsync(s => s.StudentNumber == studentNumber.Trim());

        public async Task<bool> EmailExistsAsync(string email, Guid? excludeId = null)
        {
            var query = _context.Students.Where(s => s.Email == email.Trim().ToLower());
            if (excludeId.HasValue)
                query = query.Where(s => s.Id != excludeId.Value);
            return await query.AnyAsync();
        }

        private static ReadStudentDTO MapToReadDTO(Student s) => new()
        {
            Id = s.Id,
            FirstName = s.FirstName,
            LastName = s.LastName,
            StudentNumber = s.StudentNumber,
            Email = s.Email,
            ContactNumber = s.ContactNumber,
            YearLevel = s.YearLevel,
            Program = s.Program,
            CreatedAt = s.CreatedAt
        };
    }
}