using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartSchedule.Application.Common.Exceptions;
using SmartSchedule.Application.Interfaces;

namespace SmartSchedule.Application.Services.Notifications.GetUnreadCount
{
    internal sealed class GetUnreadNotificationsCountQueryHandler(
        IApplicationDbContext context,
        IUserContextService userContextService)
        : IRequestHandler<GetUnreadNotificationsCountQuery, UnreadNotificationsCountResponse>
    {
        public async Task<UnreadNotificationsCountResponse> Handle(
            GetUnreadNotificationsCountQuery request,
            CancellationToken cancellationToken)
        {
            var userId = userContextService.GetCurrentUserId()
                ?? throw new UnauthorizedException("User is not authenticated.");

            var count = await context.Notifications
                .AsNoTracking()
                .CountAsync(n => n.UserId == userId && !n.IsRead, cancellationToken);

            return new UnreadNotificationsCountResponse(count);
        }
    }
}
