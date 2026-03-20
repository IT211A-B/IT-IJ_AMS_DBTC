using AMS_Backend.DTO.StudentDTO;
using AMS_Backend.DTO.TeacherDTO;
using AMS_Backend.Services.ServiceTeacher;
using Microsoft.AspNetCore.Mvc;

namespace AMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeachersController : ControllerBase
    {
        private readonly ITeacherService _teacherService;

        public TeachersController(ITeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        /// <summary>
        /// Get all teachers with optional pagination
        /// </summary>
        /// <param name="page">Page number (default = 1)</param>
        /// <param name="pageSize">Number of records per page (default = 10)</param>
        /// <response code="200">Returns list of teachers</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReadTeacherDTO>>> GetTeachers(int page = 1, int pageSize = 10)
        {
            var teachers = await _teacherService.GetAllTeachers();

            var paged = teachers
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            return Ok(paged);
        }


        /// <summary>
        /// Get a single teacher by ID
        /// </summary>
        /// <param name="id">Teacher ID</param>
        /// <response code="200">Teacher found</response>
        /// <response code="404">Teacher not found</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<ReadTeacherDTO>> GetTeacher(int id)
        {
            var teacher = await _teacherService.GetTeacherById(id);
            if (teacher == null) return NotFound();
            return Ok(teacher);
        }

        /// <summary>
        /// Create a new teacher
        /// </summary>
        /// <response code="201">Teacher created successfully</response>
        /// <response code="400">Invalid input</response>
        [HttpPost]
        public async Task<ActionResult<ReadTeacherDTO>> PostTeacher(CreateTeacherDTO teacherDto)
        {
            var teacher = await _teacherService.AddTeacher(teacherDto);
            return CreatedAtAction(nameof(GetTeacher), new { id = teacher.TeacherId }, teacher);
        }

        /// <summary>
        /// Update an existing teacher
        /// </summary>
        /// <param name="id">Teacher ID</param>
        /// <response code="204">Teacher updated successfully</response>
        /// <response code="404">Teacher not found</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTeacher(int id, UpdateTeacherDTO teacherDto)
        {
            await _teacherService.UpdateTeacher(id, teacherDto);
            return NoContent();
        }

        /// <summary>
        /// Delete a teacher by ID
        /// </summary>
        /// <param name="id">Teacher ID</param>
        /// <response code="204">Teacher deleted successfully</response>
        /// <response code="404">Teacher not found</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeacher(int id)
        {
            await _teacherService.DeleteTeacher(id);
            return NoContent();
        }
    }
}