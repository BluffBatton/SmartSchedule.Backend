using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartSchedule.Application.Common.Exceptions;
using SmartSchedule.Application.Interfaces;
using SmartSchedule.Domain.Enums;

namespace SmartSchedule.Application.Services.Admin.Teachers.UpdateTeacher
{
    internal sealed class UpdateTeacherCommandHandler(IApplicationDbContext context)
        : IRequestHandler<UpdateTeacherCommand>
    {
        public async Task Handle(UpdateTeacherCommand request, CancellationToken cancellationToken)
        {
            var teacher = await context.Users
                .FirstOrDefaultAsync(
                    u => u.Id == request.TeacherId && u.Role == UserRole.Teacher,
                    cancellationToken);

            if (teacher is null)
            {
                throw new NotFoundException($"Teacher with id '{request.TeacherId}' was not found.");
            }

            if (request.FirstName is not null)
            {
                var firstName = request.FirstName.Trim();
                if (string.IsNullOrWhiteSpace(firstName))
                    throw new ValidationException("First name cannot be empty.");
                teacher.FirstName = firstName;
            }

            if (request.LastName is not null)
            {
                var lastName = request.LastName.Trim();
                if (string.IsNullOrWhiteSpace(lastName))
                    throw new ValidationException("Last name cannot be empty.");
                teacher.LastName = lastName;
            }

            if (request.Email is not null)
            {
                var email = request.Email.Trim().ToLowerInvariant();

                if (string.IsNullOrWhiteSpace(email))
                    throw new ValidationException("Email cannot be empty.");

                if (!string.Equals(email, teacher.Email, StringComparison.OrdinalIgnoreCase))
                {
                    var emailTaken = await context.Users
                        .AnyAsync(
                            u => u.Id != teacher.Id && u.Email == email,
                            cancellationToken);

                    if (emailTaken)
                        throw new ConflictException("Email is already in use.");

                    teacher.Email = email;
                }
            }

            if (request.DepartmentId is not null)
            {
                if (request.DepartmentId == Guid.Empty)
                {
                    teacher.DepartmentId = null;
                }
                else
                {
                    var departmentExists = await context.Departments
                        .AnyAsync(d => d.Id == request.DepartmentId.Value, cancellationToken);

                    if (!departmentExists)
                        throw new NotFoundException(
                            $"Department with id '{request.DepartmentId}' was not found.");

                    teacher.DepartmentId = request.DepartmentId;
                }
            }

            if (request.Status is not null)
            {
                if (!Enum.TryParse<UserStatus>(request.Status, true, out var status))
                    throw new ValidationException($"Invalid status '{request.Status}'.");

                teacher.Status = status;
            }

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
