using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartSchedule.Application.Common.Exceptions;
using SmartSchedule.Application.Interfaces;
using SmartSchedule.Domain.Entities;

namespace SmartSchedule.Application.Services.Departments.CreateDepartment
{
    internal sealed class CreateDepartmentCommandHandler(IApplicationDbContext context)
        : IRequestHandler<CreateDepartmentCommand, Guid>
    {
        public async Task<Guid> Handle(
            CreateDepartmentCommand request,
            CancellationToken cancellationToken)
        {
            var name = (request.Name ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(name))
                throw new ValidationException("Department name is required.");

            if (name.Length > 150)
                throw new ValidationException("Department name must be 150 characters or fewer.");

            var nameExists = await context.Departments
                .AnyAsync(d => d.Name.ToLower() == name.ToLower(), cancellationToken);

            if (nameExists)
                throw new ConflictException($"Department with name '{name}' already exists.");

            var department = new Department
            {
                Id = Guid.NewGuid(),
                Name = name
            };

            await context.Departments.AddAsync(department, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            return department.Id;
        }
    }
}
