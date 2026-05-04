using MediatR;

namespace SmartSchedule.Application.Services.Users.Login
{
    public sealed record LoginUserCommand(string Email, string Password) : IRequest<string>
    {
    }
}
