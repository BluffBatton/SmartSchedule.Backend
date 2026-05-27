using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartSchedule.Application.Common.Exceptions;
using SmartSchedule.Application.Interfaces;

namespace SmartSchedule.Application.Services.Notifications.MarkAsRead
{
    internal sealed class MarkNotificationAsReadCommandHandler(
        IApplicationDbContext context,
        IUserContextService userContextService)
        : IRequestHandler<MarkNotificationAsReadCommand>
    {
        public async Task Handle(
            MarkNotificationAsReadCommand request,
            CancellationToken cancellationToken)
        {
            var userId = userContextService.GetCurrentUserId()
                ?? throw new UnauthorizedException("User is not authenticated.");

            var notification = await context.Notifications
                .FirstOrDefaultAsync(
                    n => n.Id == request.NotificationId,
                    cancellationToken);

            if (notification is null)
            {
                throw new NotFoundException(
                    $"Notification with id '{request.NotificationId}' was not found.");
            }

            if (notification.UserId != userId)
            {
                throw new ForbiddenException("You are not allowed to access this notification.");
            }

            if (notification.IsRead)
            {
                return;
            }

            notification.IsRead = true;
            notification.ReadAtUtc = DateTime.UtcNow;

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
