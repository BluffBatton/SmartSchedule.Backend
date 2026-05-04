using MediatR;

namespace SmartSchedule.Application.Services.Users.Register
{
    public sealed record RegisterUserCommand(
        string FirstName,
        string LastName,
        string Email,
        string Password
    ) : IRequest<Guid>;
}