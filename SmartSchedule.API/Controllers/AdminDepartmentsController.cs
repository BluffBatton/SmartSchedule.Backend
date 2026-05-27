using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartSchedule.Application.Services.Departments.CreateDepartment;
using SmartSchedule.Application.Services.Departments.DeleteDepartment;
using SmartSchedule.Application.Services.Departments.UpdateDepartment;

namespace SmartSchedule.API.Controllers
{
    [ApiController]
    [Route("api/admin/departments")]
    [Authorize(Roles = "Admin")]
    public class AdminDepartmentsController : ControllerBase
    {
        private readonly ISender _sender;

        public AdminDepartmentsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        public async Task<IActionResult> CreateDepartment(
            [FromBody] CreateDepartmentRequest request,
            CancellationToken cancellationToken)
        {
            var id = await _sender.Send(
                new CreateDepartmentCommand(request.Name ?? string.Empty),
                cancellationToken);

            return Created($"/api/departments/{id}", new { departmentId = id });
        }

        [HttpPatch("{id:guid}")]
        public async Task<IActionResult> UpdateDepartment(
            Guid id,
            [FromBody] UpdateDepartmentRequest request,
            CancellationToken cancellationToken)
        {
            await _sender.Send(
                new UpdateDepartmentCommand(id, request.Name ?? string.Empty),
                cancellationToken);

            return Ok(new { message = "Department has been updated successfully." });
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteDepartment(
            Guid id,
            CancellationToken cancellationToken)
        {
            await _sender.Send(new DeleteDepartmentCommand(id), cancellationToken);
            return Ok(new { message = "Department has been deleted successfully." });
        }
    }

    public sealed class CreateDepartmentRequest
    {
        public string? Name { get; set; }
    }

    public sealed class UpdateDepartmentRequest
    {
        public string? Name { get; set; }
    }
}
