using AMS_Backend.DTO.TeacherDTO;
using AMS_Backend.Models;
using AMS_Backend.Repositories;

namespace AMS_Backend.Services.ServiceTeacher
{
    public class TeacherService : ITeacherService
    {
        private readonly IGenericRepository<Teacher> _repository;

        public TeacherService(IGenericRepository<Teacher> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ReadTeacherDTO>> GetAllTeachers()
        {
            var teachers = await _repository.GetAllAsync();
            return teachers.Select(t => new ReadTeacherDTO
            {
                TeacherId = t.TeacherId,
                FullName = $"{t.FirstName} {t.LastName}",
                Email = t.Email
            });
        }

        public async Task<ReadTeacherDTO> GetTeacherById(int id)
        {
            var t = await _repository.GetByIdAsync(id);
            if (t == null) return null;

            return new ReadTeacherDTO
            {
                TeacherId = t.TeacherId,
                FullName = $"{t.FirstName} {t.LastName}",
                Email = t.Email
            };
        }

        public async Task<ReadTeacherDTO> AddTeacher(CreateTeacherDTO teacherDto)
        {
            var t = new Teacher
            {
                FirstName = teacherDto.FirstName,
                LastName = teacherDto.LastName,
                Email = teacherDto.Email
            };

            await _repository.AddAsync(t);

            return new ReadTeacherDTO
            {
                TeacherId = t.TeacherId,
                FullName = $"{t.FirstName} {t.LastName}",
                Email = t.Email
            };
        }

        public async Task UpdateTeacher(int id, UpdateTeacherDTO teacherDto)
        {
            var t = await _repository.GetByIdAsync(id);
            if (t == null) return;

            t.FirstName = teacherDto.FirstName;
            t.LastName = teacherDto.LastName;
            t.Email = teacherDto.Email;

            await _repository.UpdateAsync(t);
        }

        public async Task DeleteTeacher(int id)
        {
            var t = await _repository.GetByIdAsync(id);
            if (t != null)
            {
                await _repository.DeleteAsync(t);
            }
        }
    }
}