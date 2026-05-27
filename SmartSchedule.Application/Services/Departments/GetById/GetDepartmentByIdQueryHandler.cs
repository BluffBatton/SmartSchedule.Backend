using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartSchedule.Application.Common.Exceptions;
using SmartSchedule.Application.Interfaces;
using SmartSchedule.Application.Services.Departments.GetAll;
using SmartSchedule.Domain.Enums;

namespace SmartSchedule.Application.Services.Departments.GetById
{
    internal sealed class GetDepartmentByIdQueryHandler(IApplicationDbContext context)
        : IRequestHandler<GetDepartmentByIdQuery, DepartmentResponse>
    {
        public async Task<DepartmentResponse> Handle(
            GetDepartmentByIdQuery request,
            CancellationToken cancellationToken)
        {
            var department = await context.Departments
                .AsNoTracking()
                .Where(d => d.Id == request.DepartmentId)
                .Select(d => new DepartmentResponse
                {
                    Id = d.Id,
                    Name = d.Name,
                    TeachersCount = d.Users.Count(u => u.Role == UserRole.Teacher),
                    CreatedAtUtc = d.CreatedAtUtc
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (department is null)
            {
                throw new NotFoundException(
                    $"Department with id '{request.DepartmentId}' was not found.");
            }

            return department;
        }
    }
}
