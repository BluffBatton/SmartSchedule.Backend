using MediatR;

namespace SmartSchedule.Application.Services.Notifications.DeleteNotification
{
    public sealed record DeleteNotificationCommand(Guid NotificationId) : IRequest;
}
