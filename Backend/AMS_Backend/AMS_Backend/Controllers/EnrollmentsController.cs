using AMS_Backend.Common;
using AMS_Backend.DTO.EnrollmentDTO;
using AMS_Backend.Services.ServiceEnrollment;
using Microsoft.AspNetCore.Mvc;

namespace AMS_Backend.Controllers
{
    [ApiController]
    [Route("api/Enrollments")]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentsController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        /// <summary>
        /// Retrieves all enrollments in the system.
        /// </summary>
        /// <remarks>
        /// - If <b>id</b> is provided, returns a single-enrollment list. 404 if not found.<br/>
        /// - If no <b>id</b>, returns a paged list. 204 if no data.<br/>
        /// - If <b>search</b> is provided, filters by StudentName, StudentNumber, CourseCode, or CourseName.<br/>
        /// - If <b>studentId</b> is provided, returns only enrollments for that student.<br/>
        /// - If <b>courseId</b> is provided, returns only enrollments for that course.
        /// </remarks>
        [HttpGet("Get-Enrollments")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ReadEnrollmentDTO>>>> GetEnrollments(
            [FromQuery] int? page,
            [FromQuery] int? limit,
            [FromQuery] Guid? id,
            [FromQuery] string? search,
            [FromQuery] Guid? studentId,
            [FromQuery] Guid? courseId)
        {
            // Return single enrollment by ID
            if (id.HasValue)
            {
                var enrollment = await _enrollmentService.GetEnrollmentByIdAsync(id.Value);
                if (enrollment is null)
                    return NotFound(ApiResponse<IEnumerable<ReadEnrollmentDTO>>.NotFound(
                        $"Enrollment with ID '{id}' was not found."));

                return Ok(ApiResponse<IEnumerable<ReadEnrollmentDTO>>.Ok(
                    new List<ReadEnrollmentDTO> { enrollment }));
            }

            // Filter by student or course if provided
            IEnumerable<ReadEnrollmentDTO> all;
            if (studentId.HasValue)
                all = await _enrollmentService.GetEnrollmentsByStudentAsync(studentId.Value);
            else if (courseId.HasValue)
                all = await _enrollmentService.GetEnrollmentsByCourseAsync(courseId.Value);
            else
                all = await _enrollmentService.GetAllEnrollmentsAsync();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                all = all.Where(e =>
                    e.StudentName.ToLower().Contains(term) ||
                    e.StudentNumber.ToLower().Contains(term) ||
                    e.CourseCode.ToLower().Contains(term) ||
                    e.CourseName.ToLower().Contains(term));
            }

            var list = all.ToList();

            if (list.Count == 0)
                return StatusCode(204, ApiResponse<IEnumerable<ReadEnrollmentDTO>>.Ok(
                    Enumerable.Empty<ReadEnrollmentDTO>(), "No data found."));

            // Apply pagination
            if (page.HasValue && limit.HasValue && page > 0 && limit > 0)
                list = list.Skip((page.Value - 1) * limit.Value).Take(limit.Value).ToList();

            return Ok(ApiResponse<IEnumerable<ReadEnrollmentDTO>>.Ok(list));
        }

        /// <summary>
        /// Enrolls a student in a course.
        /// </summary>
        [HttpPost("Create-Enrollment")]
        public async Task<ActionResult<ApiResponse<ReadEnrollmentDTO>>> CreateEnrollment(
            [FromBody] CreateEnrollmentDTO dto)
        {
            if (!await _enrollmentService.StudentExistsAsync(dto.StudentId))
                return NotFound(ApiResponse<ReadEnrollmentDTO>.NotFound(
                    $"Student with ID '{dto.StudentId}' was not found."));

            if (!await _enrollmentService.CourseExistsAsync(dto.CourseId))
                return NotFound(ApiResponse<ReadEnrollmentDTO>.NotFound(
                    $"Course with ID '{dto.CourseId}' was not found."));

            if (await _enrollmentService.IsAlreadyEnrolledAsync(dto.StudentId, dto.CourseId))
                return Conflict(ApiResponse<ReadEnrollmentDTO>.Fail(
                    "Student is already enrolled in this course."));

            var created = await _enrollmentService.CreateEnrollmentAsync(dto);
            return Ok(ApiResponse<ReadEnrollmentDTO>.Ok(created, "Student enrolled successfully."));
        }

        /// <summary>
        /// Updates the status of an existing enrollment.
        /// </summary>
        [HttpPut("Update-Enrollment/{id:guid}")]
        public async Task<ActionResult<ApiResponse<ReadEnrollmentDTO>>> UpdateEnrollment(
            Guid id, [FromBody] UpdateEnrollmentStatusDTO dto)
        {
            var updated = await _enrollmentService.UpdateEnrollmentStatusAsync(id, dto);
            if (updated is null)
                return NotFound(ApiResponse<ReadEnrollmentDTO>.NotFound(
                    $"Enrollment with ID '{id}' was not found."));

            return Ok(ApiResponse<ReadEnrollmentDTO>.Ok(updated, "Enrollment status updated successfully."));
        }

        /// <summary>
        /// Deletes an enrollment by ID.
        /// </summary>
        [HttpDelete("Delete-Enrollment/{id:guid}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteEnrollment(Guid id)
        {
            var deleted = await _enrollmentService.DeleteEnrollmentAsync(id);
            if (!deleted)
                return NotFound(ApiResponse<object>.NotFound(
                    $"Enrollment with ID '{id}' was not found."));

            return Ok(ApiResponse<object>.Ok(null!, "Enrollment deleted successfully."));
        }
    }
}