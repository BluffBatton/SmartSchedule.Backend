using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartSchedule.Application.Interfaces;
using SmartSchedule.Domain.Entities;
using SmartSchedule.Domain.Enums;

namespace SmartSchedule.Application.Services.Users.Register
{
    internal sealed class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Guid>
    {
        private readonly IApplicationDbContext _context;
        private readonly IPasswordHasherService _passwordHasher;

        public RegisterUserCommandHandler(
            IApplicationDbContext context,
            IPasswordHasherService passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var firstName = request.FirstName.Trim();
            var lastName = request.LastName.Trim();
            var email = request.Email.Trim().ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(firstName))
                throw new InvalidOperationException("First name is required.");

            if (string.IsNullOrWhiteSpace(lastName))
                throw new InvalidOperationException("Last name is required.");

            if (string.IsNullOrWhiteSpace(email))
                throw new InvalidOperationException("Email is required.");

            if (string.IsNullOrWhiteSpace(request.Password))
                throw new InvalidOperationException("Password is required.");

            var emailAlreadyExists = await _context.Users
                .AnyAsync(u => u.Email == email, cancellationToken);

            if (emailAlreadyExists)
                throw new InvalidOperationException("User with this email already exists.");

            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Role = UserRole.Student,
                Status = UserStatus.Active
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

            await _context.Users.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return user.Id;
        }
    }
}