using AMS_Backend.Common;
using AMS_Backend.DTO.AttendanceDTO;
using AMS_Backend.Services.ServiceAttendance;
using Microsoft.AspNetCore.Mvc;

namespace AMS_Backend.Controllers
{
    [ApiController]
    [Route("api/Attendance")]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;

        public AttendanceController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        /// <summary>
        /// Retrieves attendance records in the system.
        /// </summary>
        /// <remarks>
        /// - If <b>id</b> is provided, returns that single record. 404 if not found.<br/>
        /// - If no <b>id</b>, returns a paged list. 204 if no data.<br/>
        /// - If <b>search</b> is provided, filters by StudentName, StudentNumber, or CourseCode.<br/>
        /// - If <b>studentId</b> is provided, returns all attendance for that student.<br/>
        /// - If <b>courseId</b> is provided, returns all attendance for that course.<br/>
        /// - If both <b>studentId</b> and <b>courseId</b> are provided, returns attendance for that student in that course.<br/>
        /// - If both <b>courseId</b> and <b>date</b> are provided (format: YYYY-MM-DD), returns the class attendance sheet for that date.<br/>
        /// - If both <b>studentId</b> and <b>courseId</b> are provided with <b>summary=true</b>, returns the attendance summary and percentage.
        /// </remarks>
        [HttpGet("Get-Attendance")]
        public async Task<ActionResult<ApiResponse<object>>> GetAttendance(
            [FromQuery] int? page,
            [FromQuery] int? limit,
            [FromQuery] Guid? id,
            [FromQuery] string? search,
            [FromQuery] Guid? studentId,
            [FromQuery] Guid? courseId,
            [FromQuery] string? date,
            [FromQuery] bool summary = false)
        {
            // Return single record by ID
            if (id.HasValue)
            {
                var record = await _attendanceService.GetAttendanceByIdAsync(id.Value);
                if (record is null)
                    return NotFound(ApiResponse<object>.NotFound(
                        $"Attendance record with ID '{id}' was not found."));

                return Ok(ApiResponse<object>.Ok(new List<ReadAttendanceDTO> { record }));
            }

            // Summary: student + course
            if (summary && studentId.HasValue && courseId.HasValue)
            {
                var summaryResult = await _attendanceService.GetAttendanceSummaryAsync(
                    studentId.Value, courseId.Value);

                if (summaryResult is null)
                    return NotFound(ApiResponse<object>.NotFound(
                        "Student or course not found."));

                return Ok(ApiResponse<object>.Ok(summaryResult));
            }

            // Attendance sheet: course + date
            if (courseId.HasValue && !string.IsNullOrWhiteSpace(date))
            {
                if (!DateOnly.TryParse(date, out var parsedDate))
                    return BadRequest(ApiResponse<object>.Fail(
                        "Invalid date format. Use YYYY-MM-DD (e.g. 2026-04-18)."));

                if (!await _attendanceService.CourseExistsAsync(courseId.Value))
                    return NotFound(ApiResponse<object>.NotFound(
                        $"Course with ID '{courseId}' was not found."));

                var sheet = await _attendanceService.GetAttendanceByCourseAndDateAsync(
                    courseId.Value, parsedDate);

                return Ok(ApiResponse<object>.Ok(sheet));
            }

            // Student + course
            if (studentId.HasValue && courseId.HasValue)
            {
                if (!await _attendanceService.StudentExistsAsync(studentId.Value))
                    return NotFound(ApiResponse<object>.NotFound(
                        $"Student with ID '{studentId}' was not found."));

                if (!await _attendanceService.CourseExistsAsync(courseId.Value))
                    return NotFound(ApiResponse<object>.NotFound(
                        $"Course with ID '{courseId}' was not found."));

                var records = await _attendanceService.GetAttendanceByStudentAndCourseAsync(
                    studentId.Value, courseId.Value);

                return Ok(ApiResponse<object>.Ok(records));
            }

            // All records for a student
            if (studentId.HasValue)
            {
                if (!await _attendanceService.StudentExistsAsync(studentId.Value))
                    return NotFound(ApiResponse<object>.NotFound(
                        $"Student with ID '{studentId}' was not found."));

                var records = await _attendanceService.GetAttendanceByStudentAsync(studentId.Value);
                return Ok(ApiResponse<object>.Ok(records));
            }

            // All records for a course
            if (courseId.HasValue)
            {
                if (!await _attendanceService.CourseExistsAsync(courseId.Value))
                    return NotFound(ApiResponse<object>.NotFound(
                        $"Course with ID '{courseId}' was not found."));

                var records = await _attendanceService.GetAttendanceByCourseAsync(courseId.Value);
                return Ok(ApiResponse<object>.Ok(records));
            }

            // Get all with optional search
            var all = await _attendanceService.GetAllAttendancesAsync();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                all = all.Where(a =>
                    a.StudentName.ToLower().Contains(term) ||
                    a.StudentNumber.ToLower().Contains(term) ||
                    a.CourseCode.ToLower().Contains(term) ||
                    a.CourseName.ToLower().Contains(term));
            }

            var list = all.ToList();

            if (list.Count == 0)
                return StatusCode(204, ApiResponse<object>.Ok(
                    Enumerable.Empty<ReadAttendanceDTO>(), "No data found."));

            // Apply pagination
            if (page.HasValue && limit.HasValue && page > 0 && limit > 0)
                list = list.Skip((page.Value - 1) * limit.Value).Take(limit.Value).ToList();

            return Ok(ApiResponse<object>.Ok(list));
        }

        /// <summary>
        /// Records attendance for a single student. Use bulk endpoint for an entire class.
        /// </summary>
        [HttpPost("Create-Attendance")]
        public async Task<ActionResult<ApiResponse<ReadAttendanceDTO>>> CreateAttendance(
            [FromBody] CreateAttendanceDTO dto)
        {
            if (!await _attendanceService.StudentExistsAsync(dto.StudentId))
                return NotFound(ApiResponse<ReadAttendanceDTO>.NotFound(
                    $"Student with ID '{dto.StudentId}' was not found."));

            if (!await _attendanceService.CourseExistsAsync(dto.CourseId))
                return NotFound(ApiResponse<ReadAttendanceDTO>.NotFound(
                    $"Course with ID '{dto.CourseId}' was not found."));

            if (!await _attendanceService.StudentIsEnrolledAsync(dto.StudentId, dto.CourseId))
                return BadRequest(ApiResponse<ReadAttendanceDTO>.Fail(
                    "Student is not actively enrolled in this course."));

            if (await _attendanceService.AttendanceRecordExistsAsync(dto.StudentId, dto.CourseId, dto.Date))
                return Conflict(ApiResponse<ReadAttendanceDTO>.Fail(
                    $"An attendance record for this student in this course on {dto.Date} already exists."));

            var created = await _attendanceService.CreateAttendanceAsync(dto);
            return Ok(ApiResponse<ReadAttendanceDTO>.Ok(created, "Attendance recorded successfully."));
        }

        /// <summary>
        /// Records attendance for an entire class in a single request.
        /// </summary>
        [HttpPost("Create-Attendance/bulk")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ReadAttendanceDTO>>>> BulkCreateAttendance(
            [FromBody] BulkCreateAttendanceDTO dto)
        {
            if (!await _attendanceService.CourseExistsAsync(dto.CourseId))
                return NotFound(ApiResponse<IEnumerable<ReadAttendanceDTO>>.NotFound(
                    $"Course with ID '{dto.CourseId}' was not found."));

            var errors = new List<string>();
            foreach (var entry in dto.Entries)
            {
                if (!await _attendanceService.StudentExistsAsync(entry.StudentId))
                {
                    errors.Add($"Student with ID '{entry.StudentId}' was not found.");
                    continue;
                }
                if (!await _attendanceService.StudentIsEnrolledAsync(entry.StudentId, dto.CourseId))
                    errors.Add($"Student '{entry.StudentId}' is not actively enrolled in this course.");
                else if (await _attendanceService.AttendanceRecordExistsAsync(entry.StudentId, dto.CourseId, dto.Date))
                    errors.Add($"Attendance for student '{entry.StudentId}' on {dto.Date} already exists.");
            }

            if (errors.Count > 0)
                return BadRequest(ApiResponse<IEnumerable<ReadAttendanceDTO>>.Fail(
                    "Some entries failed validation.", errors));

            var created = await _attendanceService.BulkCreateAttendanceAsync(dto);
            return Ok(ApiResponse<IEnumerable<ReadAttendanceDTO>>.Ok(created,
                $"{created.Count()} attendance records saved successfully."));
        }

        /// <summary>
        /// Updates the status or remarks of an existing attendance record.
        /// </summary>
        [HttpPut("Update-Attendance/{id:guid}")]
        public async Task<ActionResult<ApiResponse<ReadAttendanceDTO>>> UpdateAttendance(
            Guid id, [FromBody] UpdateAttendanceDTO dto)
        {
            var updated = await _attendanceService.UpdateAttendanceAsync(id, dto);
            if (updated is null)
                return NotFound(ApiResponse<ReadAttendanceDTO>.NotFound(
                    $"Attendance record with ID '{id}' was not found."));

            return Ok(ApiResponse<ReadAttendanceDTO>.Ok(updated, "Attendance record updated successfully."));
        }

        /// <summary>
        /// Deletes an attendance record by ID.
        /// </summary>
        [HttpDelete("Delete-Attendance/{id:guid}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteAttendance(Guid id)
        {
            var deleted = await _attendanceService.DeleteAttendanceAsync(id);
            if (!deleted)
                return NotFound(ApiResponse<object>.NotFound(
                    $"Attendance record with ID '{id}' was not found."));

            return Ok(ApiResponse<object>.Ok(null!, "Attendance record deleted successfully."));
        }
    }
}