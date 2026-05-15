using AMS_Backend.Common;
using AMS_Backend.DTO.StudentDTO;
using AMS_Backend.Services.ServiceStudent;
using Microsoft.AspNetCore.Mvc;

namespace AMS_Backend.Controllers
{
    [ApiController]
    [Route("api/Students")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        /// <summary>
        /// Retrieves all students in the system.
        /// </summary>
        /// <remarks>
        /// - If <b>id</b> is provided, returns a single-student list. 404 if not found.<br/>
        /// - If no <b>id</b>, returns a paged list. 204 if no data.<br/>
        /// - If <b>search</b> is provided, filters results by searching in StudentNumber, FirstName, LastName, or Email.
        /// </remarks>
        [HttpGet("Get-Students")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ReadStudentDTO>>>> GetStudents(
            [FromQuery] int? page,
            [FromQuery] int? limit,
            [FromQuery] Guid? id,
            [FromQuery] string? search)
        {
            // Return single student by ID
            if (id.HasValue)
            {
                var student = await _studentService.GetStudentByIdAsync(id.Value);
                if (student is null)
                    return NotFound(ApiResponse<IEnumerable<ReadStudentDTO>>.NotFound(
                        $"Student with ID '{id}' was not found."));

                return Ok(ApiResponse<IEnumerable<ReadStudentDTO>>.Ok(
                    new List<ReadStudentDTO> { student }));
            }

            var all = await _studentService.GetAllStudentsAsync();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                all = all.Where(s =>
                    s.StudentNumber.ToLower().Contains(term) ||
                    s.FirstName.ToLower().Contains(term) ||
                    s.LastName.ToLower().Contains(term) ||
                    s.Email.ToLower().Contains(term));
            }

            var list = all.ToList();

            if (list.Count == 0)
                return StatusCode(204, ApiResponse<IEnumerable<ReadStudentDTO>>.Ok(
                    Enumerable.Empty<ReadStudentDTO>(), "No data found."));

            // Apply pagination
            if (page.HasValue && limit.HasValue && page > 0 && limit > 0)
                list = list.Skip((page.Value - 1) * limit.Value).Take(limit.Value).ToList();

            return Ok(ApiResponse<IEnumerable<ReadStudentDTO>>.Ok(list));
        }

        /// <summary>
        /// Creates a new student.
        /// </summary>
        [HttpPost("Create-Student")]
        public async Task<ActionResult<ApiResponse<ReadStudentDTO>>> CreateStudent(
            [FromBody] CreateStudentDTO dto)
        {
            if (await _studentService.StudentNumberExistsAsync(dto.StudentNumber))
                return Conflict(ApiResponse<ReadStudentDTO>.Fail(
                    $"Student number '{dto.StudentNumber}' is already in use."));

            if (await _studentService.EmailExistsAsync(dto.Email))
                return Conflict(ApiResponse<ReadStudentDTO>.Fail(
                    $"Email '{dto.Email}' is already in use."));

            var created = await _studentService.CreateStudentAsync(dto);
            return Ok(ApiResponse<ReadStudentDTO>.Ok(created, "Student created successfully."));
        }

        /// <summary>
        /// Updates an existing student.
        /// </summary>
        [HttpPut("Update-Student/{id:guid}")]
        public async Task<ActionResult<ApiResponse<ReadStudentDTO>>> UpdateStudent(
            Guid id, [FromBody] UpdateStudentDTO dto)
        {
            if (await _studentService.EmailExistsAsync(dto.Email, excludeId: id))
                return Conflict(ApiResponse<ReadStudentDTO>.Fail(
                    $"Email '{dto.Email}' is already in use by another student."));

            var updated = await _studentService.UpdateStudentAsync(id, dto);
            if (updated is null)
                return NotFound(ApiResponse<ReadStudentDTO>.NotFound(
                    $"Student with ID '{id}' was not found."));

            return Ok(ApiResponse<ReadStudentDTO>.Ok(updated, "Student updated successfully."));
        }

        /// <summary>
        /// Deletes a student by ID.
        /// </summary>
        [HttpDelete("Delete-Student/{id:guid}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteStudent(Guid id)
        {
            var deleted = await _studentService.DeleteStudentAsync(id);
            if (!deleted)
                return NotFound(ApiResponse<object>.NotFound(
                    $"Student with ID '{id}' was not found."));

            return Ok(ApiResponse<object>.Ok(null!, "Student deleted successfully."));
        }
    }
}