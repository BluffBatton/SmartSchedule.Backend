using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartSchedule.Application.Interfaces;
using SmartSchedule.Domain.Enums;

namespace SmartSchedule.Application.Services.Departments.GetAll
{
    internal sealed class GetDepartmentsQueryHandler(IApplicationDbContext context)
        : IRequestHandler<GetDepartmentsQuery, List<DepartmentResponse>>
    {
        public async Task<List<DepartmentResponse>> Handle(
            GetDepartmentsQuery request,
            CancellationToken cancellationToken)
        {
            var query = context.Departments
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(d => d.Name.ToLower().Contains(search));
            }

            return await query
                .OrderBy(d => d.Name)
                .Select(d => new DepartmentResponse
                {
                    Id = d.Id,
                    Name = d.Name,
                    TeachersCount = d.Users.Count(u => u.Role == UserRole.Teacher),
                    CreatedAtUtc = d.CreatedAtUtc
                })
                .ToListAsync(cancellationToken);
        }
    }
}
