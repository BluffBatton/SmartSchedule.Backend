using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartSchedule.Application.Services.Admin.Dashboard;
using SmartSchedule.Application.Services.Admin.SystemControl;
using SmartSchedule.Application.Services.Users;
using SmartSchedule.Application.Services.Users.GetUsers;
using SmartSchedule.Application.Services.Users.UpdateUserRole;
using SmartSchedule.Application.Services.Users.UpdateUserStatus;

namespace SmartSchedule.API.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly ISender _sender;

        public AdminController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard(CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new GetAdminDashboardQuery(),
                cancellationToken);

            return Ok(result);
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers(
            [FromQuery] string? search,
            [FromQuery] string? role,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new GetUsersQuery(search, role),
                cancellationToken);

            return Ok(result);
        }


        [HttpPatch("users/{id:guid}/role")]
        public async Task<IActionResult> UpdateUserRole(
            Guid id,
            [FromBody] UpdateUserRoleRequest request,
            CancellationToken cancellationToken)
        {
            await _sender.Send(
                new UpdateUserRoleCommand(id, request.Role),
                cancellationToken);

            return Ok(new
            {
                message = "User role has been updated successfully."
            });
        }

        [HttpPatch("users/{id:guid}/status")]
        public async Task<IActionResult> UpdateUserStatus(
            Guid id,
            [FromBody] UpdateUserStatusRequest request,
            CancellationToken cancellationToken)
        {
            await _sender.Send(
                new UpdateUserStatusCommand(id, request.Status),
                cancellationToken);

            return Ok(new
            {
                message = "User status has been updated successfully."
            });
        }

        [HttpGet("bookings")]
        public async Task<IActionResult> GetBookings(
            [FromQuery] string? status,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new GetAdminBookingsQuery(status),
                cancellationToken);

            return Ok(result);
        }

        [HttpGet("time-slots")]
        public async Task<IActionResult> GetTimeSlots(
            [FromQuery] string? status,
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new GetAdminTimeSlotsQuery(status),
                cancellationToken);

            return Ok(result);
        }
    }

    public sealed class UpdateUserRoleRequest
    {
        public string Role { get; set; } = string.Empty;
    }

    public sealed class UpdateUserStatusRequest
    {
        public string Status { get; set; } = string.Empty;
    }
}