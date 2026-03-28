using AMS_Backend.DTO.CourseDTO;
using AMS_Backend.Models;
using AMS_Backend.Repositories;

namespace AMS_Backend.Services.ServiceCourse
{
    public class CourseService : ICourseService
    {
        private readonly IGenericRepository<Course> _repository;

        public CourseService(IGenericRepository<Course> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ReadCourseDTO>> GetAllCourses()
        {
            var courses = await _repository.GetAllAsync();
            return courses.Select(c => new ReadCourseDTO
            {
                CourseId = c.CourseId,
                Name = c.CourseName,
                Description = c.Description
            });
        }

        public async Task<ReadCourseDTO> GetCourseById(Guid id)
        {
            var c = await _repository.GetByIdAsync(id);
            if (c == null) return null;

            return new ReadCourseDTO
            {
                CourseId = c.CourseId,
                Name = c.CourseName,
                Description = c.Description
            };
        }

        public async Task<ReadCourseDTO> AddCourse(CreateCourseDTO courseDto)
        {
            var c = new Course
            {
                CourseName = courseDto.Name,
                Description = courseDto.Description
            };

            await _repository.AddAsync(c);

            return new ReadCourseDTO
            {
                CourseId = c.CourseId,
                Name = c.CourseName,
                Description = c.Description
            };
        }

        public async Task UpdateCourse(Guid id, UpdateCourseDTO courseDto)
        {
            var c = await _repository.GetByIdAsync(id);
            if (c == null) return;

            c.CourseName = courseDto.Name;
            c.Description = courseDto.Description;

            await _repository.UpdateAsync(c);
        }

        public async Task DeleteCourse(Guid id)
        {
            var c = await _repository.GetByIdAsync(id);
            if (c != null)
            {
                await _repository.DeleteAsync(c);
            }
        }
    }
}