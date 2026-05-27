using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartSchedule.Application.Services.Admin.Teachers.DeleteTeacher;
using SmartSchedule.Application.Services.Admin.Teachers.GetAdminTeacherById;
using SmartSchedule.Application.Services.Admin.Teachers.GetAdminTeachers;
using SmartSchedule.Application.Services.Admin.Teachers.UpdateTeacher;
using SmartSchedule.Application.Services.Users.Register;

namespace SmartSchedule.API.Controllers
{
    [ApiController]
    [Route("api/admin/teachers")]
    [Authorize(Roles = "Admin")]
    public class AdminTeachersController : ControllerBase
    {
        private readonly ISender _sender;

        public AdminTeachersController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> GetTeachers(
            [FromQuery] string? search,
            [FromQuery] Guid? departmentId,
            [FromQuery] string? status,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new GetAdminTeachersQuery(search, departmentId, status),
                cancellationToken);

            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetTeacherById(
            Guid id,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new GetAdminTeacherByIdQuery(id),
                cancellationToken);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTeacher(
            [FromBody] RegisterTeacherCommand command,
            CancellationToken cancellationToken)
        {
            var teacherId = await _sender.Send(command, cancellationToken);

            return CreatedAtAction(
                nameof(GetTeacherById),
                new { id = teacherId },
                new { teacherId });
        }

        [HttpPatch("{id:guid}")]
        public async Task<IActionResult> UpdateTeacher(
            Guid id,
            [FromBody] UpdateTeacherRequest request,
            CancellationToken cancellationToken)
        {
            await _sender.Send(
                new UpdateTeacherCommand
                {
                    TeacherId = id,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    DepartmentId = request.DepartmentId,
                    Status = request.Status
                },
                cancellationToken);

            return Ok(new
            {
                message = "Teacher has been updated successfully."
            });
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteTeacher(
            Guid id,
            CancellationToken cancellationToken)
        {
            await _sender.Send(new DeleteTeacherCommand(id), cancellationToken);

            return Ok(new
            {
                message = "Teacher has been deleted successfully."
            });
        }
    }

    public sealed class UpdateTeacherRequest
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public Guid? DepartmentId { get; set; }

        public string? Status { get; set; }
    }
}
