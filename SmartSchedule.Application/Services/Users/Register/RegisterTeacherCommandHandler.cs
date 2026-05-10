using MediatR;
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
            if(userId is null)
            {
                throw new ArgumentNullException(nameof(userId));
            }

            var firstName = request.FirstName.Trim();
            var lastName = request.LastName.Trim();
            var email = request.Email.Trim();
            var departmentId = request.DepartmentId;
            var password = request.Password.Trim();

            if (string.IsNullOrWhiteSpace(firstName))
                throw new InvalidOperationException("First name is required.");

            if (string.IsNullOrWhiteSpace(lastName))
                throw new InvalidOperationException("Last name is required.");

            if (string.IsNullOrWhiteSpace(email))
                throw new InvalidOperationException("Email is required.");

            if (departmentId == Guid.Empty)
                throw new InvalidOperationException("Department is required.");

            if (string.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException("Password is required.");

            var emailExists = await _context.Users.AnyAsync(u => u.Email == email, cancellationToken);
            if(emailExists){
                throw new InvalidOperationException("Email is already in use.");
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
