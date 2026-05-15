using AMS_Backend.Common;
using AMS_Backend.DTO.TeacherDTO;
using AMS_Backend.Services.ServiceTeacher;
using Microsoft.AspNetCore.Mvc;

namespace AMS_Backend.Controllers
{
    [ApiController]
    [Route("api/Teachers")]
    public class TeachersController : ControllerBase
    {
        private readonly ITeacherService _teacherService;

        public TeachersController(ITeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        /// <summary>
        /// Retrieves all teachers in the system.
        /// </summary>
        /// <remarks>
        /// - If <b>id</b> is provided, returns a single-teacher list. 404 if not found.<br/>
        /// - If no <b>id</b>, returns a paged list. 204 if no data.<br/>
        /// - If <b>search</b> is provided, filters results by searching in EmployeeNumber, FirstName, LastName, Email, or Department.
        /// </remarks>
        [HttpGet("Get-Teachers")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ReadTeacherDTO>>>> GetTeachers(
            [FromQuery] int? page,
            [FromQuery] int? limit,
            [FromQuery] Guid? id,
            [FromQuery] string? search)
        {
            // Return single teacher by ID
            if (id.HasValue)
            {
                var teacher = await _teacherService.GetTeacherByIdAsync(id.Value);
                if (teacher is null)
                    return NotFound(ApiResponse<IEnumerable<ReadTeacherDTO>>.NotFound(
                        $"Teacher with ID '{id}' was not found."));

                return Ok(ApiResponse<IEnumerable<ReadTeacherDTO>>.Ok(
                    new List<ReadTeacherDTO> { teacher }));
            }

            var all = await _teacherService.GetAllTeachersAsync();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                all = all.Where(t =>
                    t.EmployeeNumber.ToLower().Contains(term) ||
                    t.FirstName.ToLower().Contains(term) ||
                    t.LastName.ToLower().Contains(term) ||
                    t.Email.ToLower().Contains(term) ||
                    (t.Department != null && t.Department.ToLower().Contains(term)));
            }

            var list = all.ToList();

            if (list.Count == 0)
                return StatusCode(204, ApiResponse<IEnumerable<ReadTeacherDTO>>.Ok(
                    Enumerable.Empty<ReadTeacherDTO>(), "No data found."));

            // Apply pagination
            if (page.HasValue && limit.HasValue && page > 0 && limit > 0)
                list = list.Skip((page.Value - 1) * limit.Value).Take(limit.Value).ToList();

            return Ok(ApiResponse<IEnumerable<ReadTeacherDTO>>.Ok(list));
        }

        /// <summary>
        /// Creates a new teacher.
        /// </summary>
        [HttpPost("Create-Teacher")]
        public async Task<ActionResult<ApiResponse<ReadTeacherDTO>>> CreateTeacher(
            [FromBody] CreateTeacherDTO dto)
        {
            if (await _teacherService.EmployeeNumberExistsAsync(dto.EmployeeNumber))
                return Conflict(ApiResponse<ReadTeacherDTO>.Fail(
                    $"Employee number '{dto.EmployeeNumber}' is already in use."));

            if (await _teacherService.EmailExistsAsync(dto.Email))
                return Conflict(ApiResponse<ReadTeacherDTO>.Fail(
                    $"Email '{dto.Email}' is already in use."));

            var created = await _teacherService.CreateTeacherAsync(dto);
            return Ok(ApiResponse<ReadTeacherDTO>.Ok(created, "Teacher created successfully."));
        }

        /// <summary>
        /// Updates an existing teacher.
        /// </summary>
        [HttpPut("Update-Teacher/{id:guid}")]
        public async Task<ActionResult<ApiResponse<ReadTeacherDTO>>> UpdateTeacher(
            Guid id, [FromBody] UpdateTeacherDTO dto)
        {
            if (await _teacherService.EmailExistsAsync(dto.Email, excludeId: id))
                return Conflict(ApiResponse<ReadTeacherDTO>.Fail(
                    $"Email '{dto.Email}' is already in use by another teacher."));

            var updated = await _teacherService.UpdateTeacherAsync(id, dto);
            if (updated is null)
                return NotFound(ApiResponse<ReadTeacherDTO>.NotFound(
                    $"Teacher with ID '{id}' was not found."));

            return Ok(ApiResponse<ReadTeacherDTO>.Ok(updated, "Teacher updated successfully."));
        }

        /// <summary>
        /// Deletes a teacher by ID.
        /// </summary>
        [HttpDelete("Delete-Teacher/{id:guid}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteTeacher(Guid id)
        {
            var deleted = await _teacherService.DeleteTeacherAsync(id);
            if (!deleted)
                return NotFound(ApiResponse<object>.NotFound(
                    $"Teacher with ID '{id}' was not found."));

            return Ok(ApiResponse<object>.Ok(null!, "Teacher deleted successfully."));
        }
    }
}