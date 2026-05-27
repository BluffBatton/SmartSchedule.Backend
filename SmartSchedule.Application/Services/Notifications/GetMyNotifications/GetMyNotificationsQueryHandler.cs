using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartSchedule.Application.Common.Exceptions;
using SmartSchedule.Application.Common.Models;
using SmartSchedule.Application.Interfaces;

namespace SmartSchedule.Application.Services.Notifications.GetMyNotifications
{
    internal sealed class GetMyNotificationsQueryHandler(
        IApplicationDbContext context,
        IUserContextService userContextService)
        : IRequestHandler<GetMyNotificationsQuery, PagedResult<NotificationResponse>>
    {
        private const int MaxPageSize = 100;

        public async Task<PagedResult<NotificationResponse>> Handle(
            GetMyNotificationsQuery request,
            CancellationToken cancellationToken)
        {
            var userId = userContextService.GetCurrentUserId()
                ?? throw new UnauthorizedException("User is not authenticated.");

            var page = request.Page < 1 ? 1 : request.Page;
            var pageSize = request.PageSize < 1
                ? 20
                : Math.Min(request.PageSize, MaxPageSize);

            var query = context.Notifications
                .AsNoTracking()
                .Where(n => n.UserId == userId);

            if (request.OnlyUnread)
            {
                query = query.Where(n => !n.IsRead);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(n => n.CreatedAtUtc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(n => new NotificationResponse
                {
                    Id = n.Id,
                    Title = n.Title,
                    Message = n.Message,
                    Type = n.Type.ToString(),
                    IsRead = n.IsRead,
                    ReadAtUtc = n.ReadAtUtc,
                    SentAtUtc = n.SentAtUtc,
                    CreatedAtUtc = n.CreatedAtUtc,
                    BookingId = n.BookingId
                })
                .ToListAsync(cancellationToken);

            return new PagedResult<NotificationResponse>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }
    }
}
