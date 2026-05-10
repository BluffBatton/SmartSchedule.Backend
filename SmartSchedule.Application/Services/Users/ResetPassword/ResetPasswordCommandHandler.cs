using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartSchedule.Application.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace SmartSchedule.Application.Services.Users.ResetPassword
{
    internal sealed class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, bool>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPasswordHasherService _passwordHasherService;

        public ResetPasswordCommandHandler(
            IApplicationDbContext context,
            IPasswordHasherService passwordHasherService)
        {
            _context = context;
            _passwordHasherService = passwordHasherService;
        }

        public async Task<bool> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Token))
                throw new InvalidOperationException("Token is required.");

            if (string.IsNullOrWhiteSpace(request.NewPassword))
                throw new InvalidOperationException("New password is required.");

            if (request.NewPassword.Length < 6)
                throw new InvalidOperationException("Password must contain at least 6 characters.");

            var tokenHash = HashToken(request.Token);

            var resetToken = await _context.PasswordResetTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t =>
                    t.TokenHash == tokenHash &&
                    t.UsedAtUtc == null &&
                    t.ExpiresAtUtc > DateTime.UtcNow,
                    cancellationToken);

            if (resetToken is null)
                throw new InvalidOperationException("Invalid or expired reset token.");

            var user = resetToken.User;

            user.PasswordHash = _passwordHasherService.HashPassword(user, request.NewPassword);

            resetToken.UsedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        private static string HashToken(string token)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
            return Convert.ToHexString(bytes);
        }
    }
}