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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReadTeacherDTO>>> GetTeachers()
        {
            return Ok(await _teacherService.GetAllTeachers());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReadTeacherDTO>> GetTeacher(int id)
        {
            var teacher = await _teacherService.GetTeacherById(id);
            if (teacher == null) return NotFound();
            return Ok(teacher);
        }

        [HttpPost]
        public async Task<ActionResult<ReadTeacherDTO>> PostTeacher(CreateTeacherDTO teacherDto)
        {
            var teacher = await _teacherService.AddTeacher(teacherDto);
            return CreatedAtAction(nameof(GetTeacher), new { id = teacher.TeacherId }, teacher);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTeacher(int id, UpdateTeacherDTO teacherDto)
        {
            await _teacherService.UpdateTeacher(id, teacherDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeacher(int id)
        {
            await _teacherService.DeleteTeacher(id);
            return NoContent();
        }
    }
}