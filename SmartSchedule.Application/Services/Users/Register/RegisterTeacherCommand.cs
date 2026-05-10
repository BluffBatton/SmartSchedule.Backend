using MediatR;

namespace SmartSchedule.Application.Services.Users.Register
{
    public sealed record RegisterTeacherCommand(
        string FirstName,
        string LastName,
        string Email,
        Guid DepartmentId,
        string Password
    ) : IRequest<Guid>;
}
