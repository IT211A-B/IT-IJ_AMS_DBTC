using AMS_Backend.DTO.StudentDTO;
using AMS_Backend.Services.ServiceStudent;
using Microsoft.AspNetCore.Mvc;

namespace AMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        /// <summary>
        /// Get all students with optional pagination
        /// </summary>
        /// <param name="page">Page number (default = 1)</param>
        /// <param name="pageSize">Number of records per page (default = 10)</param>
        /// <response code="200">Returns list of students</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReadStudentDTO>>> GetStudents(int page = 1, int pageSize = 10)
        {
            var students = await _studentService.GetAllStudents();

            var paged = students
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            return Ok(paged);
        }

        // GET BY ID
        /// <summary>
        /// Get a single student by ID
        /// </summary>
        /// <param name="id">Student ID</param>
        /// <response code="200">Student found</response>
        /// <response code="404">Student not found</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<ReadStudentDTO>> GetStudent(int id)
        {
            var student = await _studentService.GetStudentById(id);

            if (student == null)
                return NotFound();

            return Ok(student);
        }

        // CREATE
        /// <summary>
        /// Create a new student
        /// </summary>
        /// <response code="201">Student created successfully</response>
        /// <response code="400">Invalid input</response>
        [HttpPost]
        public async Task<ActionResult<ReadStudentDTO>> PostStudent(CreateStudentDTO dto)
        {
            var newStudent = await _studentService.AddStudent(dto);

            return CreatedAtAction(nameof(GetStudent), new { id = newStudent.StudentId }, newStudent);
        }

        // UPDATE
        /// <summary>
        /// Update an existing student
        /// </summary>
        /// <param name="id">Student ID</param>
        /// <response code="204">Student updated successfully</response>
        /// <response code="404">Student not found</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudent(int id, UpdateStudentDTO dto)
        {
            await _studentService.UpdateStudent(id, dto);
            return NoContent();
        }

        // DELETE
        /// <summary>
        /// Delete a student by ID
        /// </summary>
        /// <param name="id">Student ID</param>
        /// <response code="204">Student deleted successfully</response>
        /// <response code="404">Student not found</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            await _studentService.DeleteStudent(id);
            return NoContent();
        }
    }
}