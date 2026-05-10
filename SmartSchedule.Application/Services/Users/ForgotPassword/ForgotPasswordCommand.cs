using MediatR;

namespace SmartSchedule.Application.Services.Users.ForgotPassword
{
    public sealed record ForgotPasswordCommand(string Email) : IRequest<string?>;
}
