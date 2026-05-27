using MediatR;

namespace SmartSchedule.Application.Services.Notifications.MarkAsRead
{
    public sealed record MarkNotificationAsReadCommand(Guid NotificationId) : IRequest;
}
