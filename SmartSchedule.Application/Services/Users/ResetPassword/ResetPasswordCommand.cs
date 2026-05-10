using MediatR;

namespace SmartSchedule.Application.Services.Users.ResetPassword
{
    public sealed record ResetPasswordCommand(
        string Token,
        string NewPassword
    ) : IRequest<bool>;
}