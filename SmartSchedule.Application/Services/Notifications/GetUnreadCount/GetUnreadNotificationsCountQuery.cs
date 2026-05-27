using MediatR;

namespace SmartSchedule.Application.Services.Notifications.GetUnreadCount
{
    public sealed record GetUnreadNotificationsCountQuery : IRequest<UnreadNotificationsCountResponse>;

    public sealed record UnreadNotificationsCountResponse(int UnreadCount);
}
