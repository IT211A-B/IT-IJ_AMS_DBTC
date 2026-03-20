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

        // GET ALL
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReadStudentDTO>>> GetStudents()
        {
            return Ok(await _studentService.GetAllStudents());
        }

        // GET BY ID
        [HttpGet("{id}")]
        public async Task<ActionResult<ReadStudentDTO>> GetStudent(int id)
        {
            var student = await _studentService.GetStudentById(id);

            if (student == null)
                return NotFound();

            return Ok(student);
        }

        // CREATE
        [HttpPost]
        public async Task<ActionResult<ReadStudentDTO>> PostStudent(CreateStudentDTO dto)
        {
            var newStudent = await _studentService.AddStudent(dto);

            return CreatedAtAction(nameof(GetStudent), new { id = newStudent.StudentId }, newStudent);
        }

        // UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudent(int id, UpdateStudentDTO dto)
        {
            await _studentService.UpdateStudent(id, dto);
            return NoContent();
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            await _studentService.DeleteStudent(id);
            return NoContent();
        }
    }
}