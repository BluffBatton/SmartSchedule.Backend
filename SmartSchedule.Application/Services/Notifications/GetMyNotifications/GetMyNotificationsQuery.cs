using MediatR;
using SmartSchedule.Application.Common.Models;

namespace SmartSchedule.Application.Services.Notifications.GetMyNotifications
{
    public sealed record GetMyNotificationsQuery(
        int Page,
        int PageSize,
        bool OnlyUnread
    ) : IRequest<PagedResult<NotificationResponse>>;
}
