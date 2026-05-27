using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartSchedule.Application.Services.Notifications.DeleteNotification;
using SmartSchedule.Application.Services.Notifications.GetMyNotifications;
using SmartSchedule.Application.Services.Notifications.GetUnreadCount;
using SmartSchedule.Application.Services.Notifications.MarkAllAsRead;
using SmartSchedule.Application.Services.Notifications.MarkAsRead;

namespace SmartSchedule.API.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly ISender _sender;

        public NotificationsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyNotifications(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] bool onlyUnread = false,
            CancellationToken cancellationToken = default)
        {
            var result = await _sender.Send(
                new GetMyNotificationsQuery(page, pageSize, onlyUnread),
                cancellationToken);

            return Ok(result);
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount(CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new GetUnreadNotificationsCountQuery(),
                cancellationToken);

            return Ok(result);
        }

        [HttpPatch("{id:guid}/read")]
        public async Task<IActionResult> MarkAsRead(
            Guid id,
            CancellationToken cancellationToken)
        {
            await _sender.Send(new MarkNotificationAsReadCommand(id), cancellationToken);

            return Ok(new
            {
                message = "Notification marked as read."
            });
        }

        [HttpPatch("read-all")]
        public async Task<IActionResult> MarkAllAsRead(CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new MarkAllNotificationsAsReadCommand(),
                cancellationToken);

            return Ok(new
            {
                message = "Notifications marked as read.",
                updatedCount = result.UpdatedCount
            });
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(
            Guid id,
            CancellationToken cancellationToken)
        {
            await _sender.Send(new DeleteNotificationCommand(id), cancellationToken);

            return Ok(new
            {
                message = "Notification deleted."
            });
        }
    }
}
