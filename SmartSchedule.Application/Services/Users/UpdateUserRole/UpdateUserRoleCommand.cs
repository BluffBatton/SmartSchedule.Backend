using MediatR;

namespace SmartSchedule.Application.Services.Users.UpdateUserRole
{
    public sealed record UpdateUserRoleCommand(
        Guid UserId,
        string Role
    ) : IRequest;
}
