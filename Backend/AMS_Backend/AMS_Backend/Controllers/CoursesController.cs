using AMS_Backend.DTO.CourseDTO;
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReadCourseDTO>>> GetCourses()
        {
            return Ok(await _courseService.GetAllCourses());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReadCourseDTO>> GetCourse(int id)
        {
            var course = await _courseService.GetCourseById(id);
            if (course == null) return NotFound();
            return Ok(course);
        }

        [HttpPost]
        public async Task<ActionResult<ReadCourseDTO>> PostCourse(CreateCourseDTO courseDto)
        {
            var course = await _courseService.AddCourse(courseDto);
            return CreatedAtAction(nameof(GetCourse), new { id = course.CourseId }, course);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCourse(int id, UpdateCourseDTO courseDto)
        {
            await _courseService.UpdateCourse(id, courseDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            await _courseService.DeleteCourse(id);
            return NoContent();
        }
    }
}