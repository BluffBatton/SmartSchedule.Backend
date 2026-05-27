using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartSchedule.Application.Common.Exceptions;
using SmartSchedule.Application.Interfaces;

namespace SmartSchedule.Application.Services.Departments.UpdateDepartment
{
    internal sealed class UpdateDepartmentCommandHandler(IApplicationDbContext context)
        : IRequestHandler<UpdateDepartmentCommand>
    {
        public async Task Handle(
            UpdateDepartmentCommand request,
            CancellationToken cancellationToken)
        {
            var department = await context.Departments
                .FirstOrDefaultAsync(d => d.Id == request.DepartmentId, cancellationToken);

            if (department is null)
            {
                throw new NotFoundException(
                    $"Department with id '{request.DepartmentId}' was not found.");
            }

            var name = (request.Name ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(name))
                throw new ValidationException("Department name is required.");

            if (name.Length > 150)
                throw new ValidationException("Department name must be 150 characters or fewer.");

            if (!string.Equals(name, department.Name, StringComparison.OrdinalIgnoreCase))
            {
                var nameExists = await context.Departments
                    .AnyAsync(
                        d => d.Id != department.Id && d.Name.ToLower() == name.ToLower(),
                        cancellationToken);

                if (nameExists)
                    throw new ConflictException($"Department with name '{name}' already exists.");
            }

            department.Name = name;

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
