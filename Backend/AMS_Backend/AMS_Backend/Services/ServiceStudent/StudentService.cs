using AMS_Backend.DTO.StudentDTO;
using AMS_Backend.Models;
using AMS_Backend.Repositories;

namespace AMS_Backend.Services.ServiceStudent
{
    public class StudentService : IStudentService
    {
        private readonly IGenericRepository<Student> _repository;

        public StudentService(IGenericRepository<Student> repository)
        {
            _repository = repository;
        }

        // GET ALL
        public async Task<IEnumerable<ReadStudentDTO>> GetAllStudents()
        {
            var students = await _repository.GetAllAsync();

            return students.Select(s => new ReadStudentDTO
            {
                StudentId = s.StudentId,
                FullName = s.FirstName + " " + s.LastName,
                Email = s.Email
            });
        }

        // GET BY ID
        public async Task<ReadStudentDTO> GetStudentById(Guid id)
        {
            var s = await _repository.GetByIdAsync(id);

            if (s == null) return null;

            return new ReadStudentDTO
            {
                StudentId = s.StudentId,
                FullName = s.FirstName + " " + s.LastName,
                Email = s.Email
            };
        }

        // CREATE
        public async Task<ReadStudentDTO> AddStudent(CreateStudentDTO dto)
        {
            var student = new Student
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email
            };

            await _repository.AddAsync(student);

            return new ReadStudentDTO
            {
                StudentId = student.StudentId,
                FullName = student.FirstName + " " + student.LastName,
                Email = student.Email
            };
        }

        // UPDATE
        public async Task UpdateStudent(Guid id, UpdateStudentDTO dto)
        {
            var student = await _repository.GetByIdAsync(id);

            if (student != null)
            {
                student.FirstName = dto.FirstName;
                student.LastName = dto.LastName;
                student.Email = dto.Email;

                await _repository.UpdateAsync(student);
            }
        }

        // DELETE
        public async Task DeleteStudent(Guid id)
        {
            var student = await _repository.GetByIdAsync(id);

            if (student != null)
            {
                await _repository.DeleteAsync(student);
            }
        }
    }
}