using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartSchedule.Application.Common.Exceptions;
using SmartSchedule.Application.Interfaces;

namespace SmartSchedule.Application.Services.Departments.DeleteDepartment
{
    internal sealed class DeleteDepartmentCommandHandler(IApplicationDbContext context)
        : IRequestHandler<DeleteDepartmentCommand>
    {
        public async Task Handle(
            DeleteDepartmentCommand request,
            CancellationToken cancellationToken)
        {
            var department = await context.Departments
                .FirstOrDefaultAsync(d => d.Id == request.DepartmentId, cancellationToken);

            if (department is null)
            {
                throw new NotFoundException(
                    $"Department with id '{request.DepartmentId}' was not found.");
            }

            var hasUsers = await context.Users
                .AnyAsync(u => u.DepartmentId == request.DepartmentId, cancellationToken);

            if (hasUsers)
            {
                throw new ConflictException(
                    "Cannot delete a department that still has users assigned. " +
                    "Reassign or remove them first.");
            }

            context.Departments.Remove(department);

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
