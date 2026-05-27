using MediatR;
using SmartSchedule.Application.Common.Exceptions;
using SmartSchedule.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using SmartSchedule.Domain.Entities;

namespace SmartSchedule.Application.Services.Users.Register
{
    internal sealed class RegisterTeacherCommandHandler : IRequestHandler<RegisterTeacherCommand, Guid>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContextService _userContextService;
        private readonly IPasswordHasherService _passwordHasherService;
        public RegisterTeacherCommandHandler(IApplicationDbContext context, IUserContextService userContextService, IPasswordHasherService passwordHasherService)
        {
            _context = context;
            _userContextService = userContextService;
            _passwordHasherService = passwordHasherService;
        }

        public async Task<Guid> Handle(RegisterTeacherCommand request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.GetCurrentUserId();
            if (userId is null)
            {
                throw new UnauthorizedException("User is not authenticated.");
            }

            var firstName = request.FirstName.Trim();
            var lastName = request.LastName.Trim();
            var email = request.Email.Trim().ToLowerInvariant();
            var departmentId = request.DepartmentId;
            var password = request.Password.Trim();

            if (string.IsNullOrWhiteSpace(firstName))
                throw new ValidationException("First name is required.");

            if (string.IsNullOrWhiteSpace(lastName))
                throw new ValidationException("Last name is required.");

            if (string.IsNullOrWhiteSpace(email))
                throw new ValidationException("Email is required.");

            if (departmentId == Guid.Empty)
                throw new ValidationException("Department is required.");

            if (string.IsNullOrWhiteSpace(password))
                throw new ValidationException("Password is required.");

            var departmentExists = await _context.Departments
                .AnyAsync(d => d.Id == departmentId, cancellationToken);

            if (!departmentExists)
                throw new NotFoundException($"Department with id '{departmentId}' was not found.");

            var emailExists = await _context.Users.AnyAsync(u => u.Email == email, cancellationToken);
            if (emailExists)
            {
                throw new ConflictException("Email is already in use.");
            }

            var teacher = new User
            {
                Id = Guid.NewGuid(),
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                DepartmentId = departmentId,
                Role = Domain.Enums.UserRole.Teacher,
                Status = Domain.Enums.UserStatus.Active,
                CreatedByAdminId = userId
            };

            teacher.PasswordHash = _passwordHasherService.HashPassword(teacher, password);

            await _context.Users.AddAsync(teacher, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return teacher.Id;
        }
    }
}
