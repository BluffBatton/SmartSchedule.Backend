using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartSchedule.Application.Common.Exceptions;
using SmartSchedule.Application.Interfaces;
using SmartSchedule.Domain.Enums;

namespace SmartSchedule.Application.Services.Users.Login
{
    internal sealed class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, string>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPasswordHasherService _passwordHasherService;
        private readonly IJwtService _jwtService;

        public LoginUserCommandHandler(
            IApplicationDbContext context,
            IPasswordHasherService passwordHasherService,
            IJwtService jwtService)
        {
            _context = context;
            _passwordHasherService = passwordHasherService;
            _jwtService = jwtService;
        }

        public async Task<string> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var email = request.Email.Trim().ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(email))
                throw new ValidationException("Email is required.");

            if (string.IsNullOrWhiteSpace(request.Password))
                throw new ValidationException("Password is required.");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email, cancellationToken);

            if (user is null)
                throw new UnauthorizedException("Invalid email or password.");

            if (user.Status == UserStatus.Blocked)
                throw new ForbiddenException("User is blocked.");

            var isPasswordValid = _passwordHasherService.VerifyPassword(
                user,
                request.Password,
                user.PasswordHash);

            if (!isPasswordValid)
                throw new UnauthorizedException("Invalid email or password.");

            user.LastLoginAtUtc = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            var accessToken = _jwtService.GenerateAccessToken(user);

            return accessToken;
        }
    }
}