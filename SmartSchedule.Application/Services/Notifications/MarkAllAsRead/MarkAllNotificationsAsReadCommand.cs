using MediatR;

namespace SmartSchedule.Application.Services.Notifications.MarkAllAsRead
{
    public sealed record MarkAllNotificationsAsReadCommand : IRequest<MarkAllNotificationsAsReadResponse>;

    public sealed record MarkAllNotificationsAsReadResponse(int UpdatedCount);
}
