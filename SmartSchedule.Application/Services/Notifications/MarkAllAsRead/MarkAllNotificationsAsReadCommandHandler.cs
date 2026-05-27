using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartSchedule.Application.Common.Exceptions;
using SmartSchedule.Application.Interfaces;

namespace SmartSchedule.Application.Services.Notifications.MarkAllAsRead
{
    internal sealed class MarkAllNotificationsAsReadCommandHandler(
        IApplicationDbContext context,
        IUserContextService userContextService)
        : IRequestHandler<MarkAllNotificationsAsReadCommand, MarkAllNotificationsAsReadResponse>
    {
        public async Task<MarkAllNotificationsAsReadResponse> Handle(
            MarkAllNotificationsAsReadCommand request,
            CancellationToken cancellationToken)
        {
            var userId = userContextService.GetCurrentUserId()
                ?? throw new UnauthorizedException("User is not authenticated.");

            var now = DateTime.UtcNow;

            var updated = await context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ExecuteUpdateAsync(
                    s => s
                        .SetProperty(n => n.IsRead, true)
                        .SetProperty(n => n.ReadAtUtc, now)
                        .SetProperty(n => n.UpdatedAtUtc, now),
                    cancellationToken);

            return new MarkAllNotificationsAsReadResponse(updated);
        }
    }
}
