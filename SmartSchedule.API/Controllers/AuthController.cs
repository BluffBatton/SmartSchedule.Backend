using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartSchedule.Application.Services.Users.ForgotPassword;
using SmartSchedule.Application.Services.Users.Login;
using SmartSchedule.Application.Services.Users.Register;
using SmartSchedule.Application.Services.Users.ResetPassword;

namespace SmartSchedule.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly IConfiguration _configuration;

        public AuthController(ISender sender, IConfiguration configuration)
        {
            _sender = sender;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromBody] RegisterUserCommand command,
            CancellationToken cancellationToken)
        {
            var userId = await _sender.Send(command, cancellationToken);

            return Ok(new
            {
                userId
            });
        }

        [HttpPost("register-teacher")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegisterTeacher(
            [FromBody] RegisterTeacherCommand command,
            CancellationToken cancellationToken)
        {
            var userId = await _sender.Send(command, cancellationToken);
            return Ok(new
            {
                userId
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginUserCommand command,
            CancellationToken cancellationToken)
        {
            var accessToken = await _sender.Send(command, cancellationToken);

            return Ok(new
            {
                accessToken
            });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(
            [FromBody] ForgotPasswordCommand command,
            CancellationToken cancellationToken)
        {
            var token = await _sender.Send(command, cancellationToken);

            if (token is null)
            {
                return Ok(new
                {
                    message = "If user with this email exists, password reset instructions were created."
                });
            }

            var frontendUrl = _configuration["Frontend:BaseUrl"] ?? "http://localhost:5173";

            var resetLink = $"{frontendUrl}/reset-password?token={Uri.EscapeDataString(token)}";

            return Ok(new
            {
                message = "Password reset link was created.",
                resetLink
            });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(
            [FromBody] ResetPasswordCommand command,
            CancellationToken cancellationToken)
        {
            await _sender.Send(command, cancellationToken);

            return Ok(new
            {
                message = "Password has been reset successfully."
            });
        }
    }
}