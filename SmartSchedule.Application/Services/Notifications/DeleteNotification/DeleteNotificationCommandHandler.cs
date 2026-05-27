using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartSchedule.Application.Common.Exceptions;
using SmartSchedule.Application.Interfaces;

namespace SmartSchedule.Application.Services.Notifications.DeleteNotification
{
    internal sealed class DeleteNotificationCommandHandler(
        IApplicationDbContext context,
        IUserContextService userContextService)
        : IRequestHandler<DeleteNotificationCommand>
    {
        public async Task Handle(
            DeleteNotificationCommand request,
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
                throw new ForbiddenException("You are not allowed to delete this notification.");
            }

            context.Notifications.Remove(notification);

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
