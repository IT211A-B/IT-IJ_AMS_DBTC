using AMS_Backend.Common;
using AMS_Backend.DTO.CourseDTO;
using AMS_Backend.Services.ServiceCourse;
using Microsoft.AspNetCore.Mvc;

namespace AMS_Backend.Controllers
{
    [ApiController]
    [Route("api/Courses")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CoursesController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        /// <summary>
        /// Retrieves all courses in the system.
        /// </summary>
        /// <remarks>
        /// - If <b>id</b> is provided, returns a single-course list. 404 if not found.<br/>
        /// - If no <b>id</b>, returns a paged list. 204 if no data.<br/>
        /// - If <b>search</b> is provided, filters results by searching in CourseCode, CourseName, or TeacherName.<br/>
        /// - If <b>teacherId</b> is provided, returns only courses assigned to that teacher.
        /// </remarks>
        [HttpGet("Get-Courses")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ReadCourseDTO>>>> GetCourses(
            [FromQuery] int? page,
            [FromQuery] int? limit,
            [FromQuery] Guid? id,
            [FromQuery] string? search,
            [FromQuery] Guid? teacherId)
        {
            // Return single course by ID
            if (id.HasValue)
            {
                var course = await _courseService.GetCourseByIdAsync(id.Value);
                if (course is null)
                    return NotFound(ApiResponse<IEnumerable<ReadCourseDTO>>.NotFound(
                        $"Course with ID '{id}' was not found."));

                return Ok(ApiResponse<IEnumerable<ReadCourseDTO>>.Ok(
                    new List<ReadCourseDTO> { course }));
            }

            // Filter by teacher if provided
            IEnumerable<ReadCourseDTO> all = teacherId.HasValue
                ? await _courseService.GetCoursesByTeacherAsync(teacherId.Value)
                : await _courseService.GetAllCoursesAsync();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                all = all.Where(c =>
                    c.CourseCode.ToLower().Contains(term) ||
                    c.CourseName.ToLower().Contains(term) ||
                    c.TeacherName.ToLower().Contains(term));
            }

            var list = all.ToList();

            if (list.Count == 0)
                return StatusCode(204, ApiResponse<IEnumerable<ReadCourseDTO>>.Ok(
                    Enumerable.Empty<ReadCourseDTO>(), "No data found."));

            // Apply pagination
            if (page.HasValue && limit.HasValue && page > 0 && limit > 0)
                list = list.Skip((page.Value - 1) * limit.Value).Take(limit.Value).ToList();

            return Ok(ApiResponse<IEnumerable<ReadCourseDTO>>.Ok(list));
        }

        /// <summary>
        /// Creates a new course.
        /// </summary>
        [HttpPost("Create-Course")]
        public async Task<ActionResult<ApiResponse<ReadCourseDTO>>> CreateCourse(
            [FromBody] CreateCourseDTO dto)
        {
            if (!await _courseService.TeacherExistsAsync(dto.TeacherId))
                return NotFound(ApiResponse<ReadCourseDTO>.NotFound(
                    $"Teacher with ID '{dto.TeacherId}' was not found."));

            var created = await _courseService.CreateCourseAsync(dto);
            return Ok(ApiResponse<ReadCourseDTO>.Ok(created, "Course created successfully."));
        }

        /// <summary>
        /// Updates an existing course.
        /// </summary>
        [HttpPut("Update-Course/{id:guid}")]
        public async Task<ActionResult<ApiResponse<ReadCourseDTO>>> UpdateCourse(
            Guid id, [FromBody] UpdateCourseDTO dto)
        {
            if (!await _courseService.TeacherExistsAsync(dto.TeacherId))
                return NotFound(ApiResponse<ReadCourseDTO>.NotFound(
                    $"Teacher with ID '{dto.TeacherId}' was not found."));

            var updated = await _courseService.UpdateCourseAsync(id, dto);
            if (updated is null)
                return NotFound(ApiResponse<ReadCourseDTO>.NotFound(
                    $"Course with ID '{id}' was not found."));

            return Ok(ApiResponse<ReadCourseDTO>.Ok(updated, "Course updated successfully."));
        }

        /// <summary>
        /// Deletes a course by ID.
        /// </summary>
        [HttpDelete("Delete-Course/{id:guid}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteCourse(Guid id)
        {
            var deleted = await _courseService.DeleteCourseAsync(id);
            if (!deleted)
                return NotFound(ApiResponse<object>.NotFound(
                    $"Course with ID '{id}' was not found."));

            return Ok(ApiResponse<object>.Ok(null!, "Course deleted successfully."));
        }
    }
}