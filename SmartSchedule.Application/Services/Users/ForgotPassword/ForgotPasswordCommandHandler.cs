using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartSchedule.Application.Interfaces;
using SmartSchedule.Domain.Entities;
using System.Security.Cryptography;
using System.Text;

namespace SmartSchedule.Application.Services.Users.ForgotPassword
{
    internal sealed class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, string?>
    {
        private readonly IApplicationDbContext _context;
        public ForgotPasswordCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<string?> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var email = request.Email.Trim().ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new InvalidOperationException("Email is required");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

            if (user == null)
            {
                return null;
            }

            var rawToken = GenerateToken();
            var tokenHash = HashToken(rawToken);

            var resetToken = new PasswordResetToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                TokenHash = tokenHash,
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(30),
                UsedAtUtc = null
            };

            await _context.PasswordResetTokens.AddAsync(resetToken, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return rawToken;
        }

        private static string GenerateToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(64);

            return Convert.ToBase64String(bytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
        }

        private static string HashToken(string token)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
            return Convert.ToHexString(bytes);
        }
    }
}
