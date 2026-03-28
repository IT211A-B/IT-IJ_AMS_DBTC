using AMS_Backend.DTO.CourseDTO;
using AMS_Backend.DTO.StudentDTO;
using AMS_Backend.Services.ServiceCourse;
using Microsoft.AspNetCore.Mvc;

namespace AMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CoursesController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        /// <summary>
        /// Get all courses with optional pagination
        /// </summary>
        /// <param name="page">Page number (default = 1)</param>
        /// <param name="pageSize">Number of records per page (default = 10)</param>
        /// <response code="200">Returns list of courses</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReadCourseDTO>>> GetCourse(int page = 1, int pageSize = 10)
        {
            var courses = await _courseService.GetAllCourses();

            var paged = courses 
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            return Ok(paged);
        }

        /// <summary>
        /// Get a single course by ID
        /// </summary>
        /// <param name="id">Course ID</param>
        /// <response code="200">Course found</response>
        /// <response code="404">Course not found</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<ReadCourseDTO>> GetCourse(Guid id)
        {
            var course = await _courseService.GetCourseById(id);
            if (course == null) return NotFound();
            return Ok(course);
        }

        /// <summary>
        /// Create a new course
        /// </summary>
        /// <response code="201">Course created successfully</response>
        /// <response code="400">Invalid input</response>
        [HttpPost]
        public async Task<ActionResult<ReadCourseDTO>> PostCourse(CreateCourseDTO courseDto)
        {
            var course = await _courseService.AddCourse(courseDto);
            return CreatedAtAction(nameof(GetCourse), new { id = course.CourseId }, course);
        }

        /// <summary>
        /// Update an existing course
        /// </summary>
        /// <param name="id">Course ID</param>
        /// <response code="204">Course updated successfully</response>
        /// <response code="404">Course not found</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCourse(Guid id, UpdateCourseDTO courseDto)
        {
            await _courseService.UpdateCourse(id, courseDto);
            return NoContent();
        }

        /// <summary>
        /// Delete a course by ID
        /// </summary>
        /// <param name="id">Course ID</param>
        /// <response code="204">Course deleted successfully</response>
        /// <response code="404">Course not found</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(Guid id)
        {
            await _courseService.DeleteCourse(id);
            return NoContent();
        }
    }
}