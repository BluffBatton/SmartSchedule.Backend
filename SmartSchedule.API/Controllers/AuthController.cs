using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartSchedule.Application.Services.Users.Login;
using SmartSchedule.Application.Services.Users.Register;

namespace SmartSchedule.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ISender _sender;

        public AuthController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromBody] RegisterUserCommand command,
            CancellationToken cancellationToken)
        {
            try
            {
                var userId = await _sender.Send(command, cancellationToken);

                return Ok(new
                {
                    userId
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginUserCommand command,
            CancellationToken cancellationToken)
        {
            try
            {
                var accessToken = await _sender.Send(command, cancellationToken);

                return Ok(new
                {
                    accessToken
                });
            }
            catch (InvalidOperationException ex)
            {
                return Unauthorized(new
                {
                    message = ex.Message
                });
            }
        }
    }
}