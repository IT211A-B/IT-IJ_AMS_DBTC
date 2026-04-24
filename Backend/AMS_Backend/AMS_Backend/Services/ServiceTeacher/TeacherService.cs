using AMS_Backend.Data;
using AMS_Backend.DTO.TeacherDTO;
using AMS_Backend.Models;
using AMS_Backend.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AMS_Backend.Services.ServiceTeacher
{
    public class TeacherService : ITeacherService
    {
        private readonly IGenericRepository<Teacher> _repo;
        private readonly ApplicationDbContext _context;

        public TeacherService(IGenericRepository<Teacher> repo, ApplicationDbContext context)
        {
            _repo = repo;
            _context = context;
        }

        public async Task<IEnumerable<ReadTeacherDTO>> GetAllTeachersAsync()
        {
            var teachers = await _repo.GetAllAsync();
            return teachers.Select(MapToReadDTO);
        }

        public async Task<ReadTeacherDTO?> GetTeacherByIdAsync(Guid id)
        {
            var teacher = await _repo.GetByIdAsync(id);
            return teacher is null ? null : MapToReadDTO(teacher);
        }

        public async Task<ReadTeacherDTO> CreateTeacherAsync(CreateTeacherDTO dto)
        {
            var teacher = new Teacher
            {
                FirstName = dto.FirstName.Trim(),
                LastName = dto.LastName.Trim(),
                EmployeeNumber = dto.EmployeeNumber.Trim(),
                Email = dto.Email.Trim().ToLower(),
                ContactNumber = dto.ContactNumber?.Trim(),
                Department = dto.Department?.Trim()
            };
            var created = await _repo.CreateAsync(teacher);
            return MapToReadDTO(created);
        }

        public async Task<ReadTeacherDTO?> UpdateTeacherAsync(Guid id, UpdateTeacherDTO dto)
        {
            var teacher = await _repo.GetByIdAsync(id);
            if (teacher is null) return null;

            teacher.FirstName = dto.FirstName.Trim();
            teacher.LastName = dto.LastName.Trim();
            teacher.Email = dto.Email.Trim().ToLower();
            teacher.ContactNumber = dto.ContactNumber?.Trim();
            teacher.Department = dto.Department?.Trim();

            var updated = await _repo.UpdateAsync(teacher);
            return MapToReadDTO(updated);
        }

        public async Task<bool> DeleteTeacherAsync(Guid id)
            => await _repo.DeleteAsync(id);

        public async Task<bool> EmployeeNumberExistsAsync(string employeeNumber)
            => await _context.Teachers.AnyAsync(t => t.EmployeeNumber == employeeNumber.Trim());

        public async Task<bool> EmailExistsAsync(string email, Guid? excludeId = null)
        {
            var query = _context.Teachers.Where(t => t.Email == email.Trim().ToLower());
            if (excludeId.HasValue)
                query = query.Where(t => t.Id != excludeId.Value);
            return await query.AnyAsync();
        }

        private static ReadTeacherDTO MapToReadDTO(Teacher t) => new()
        {
            Id = t.Id,
            FirstName = t.FirstName,
            LastName = t.LastName,
            EmployeeNumber = t.EmployeeNumber,
            Email = t.Email,
            ContactNumber = t.ContactNumber,
            Department = t.Department,
            CreatedAt = t.CreatedAt
        };
    }
}