using MediatR;

namespace SmartSchedule.Application.Services.Users.UpdateUserStatus
{
    public sealed record UpdateUserStatusCommand(
        Guid UserId,
        string Status
    ) : IRequest;
}
