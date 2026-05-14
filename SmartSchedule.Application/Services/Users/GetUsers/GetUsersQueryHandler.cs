using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartSchedule.Application.Interfaces;
using SmartSchedule.Domain.Enums;


namespace SmartSchedule.Application.Services.Users.GetUsers
{
    internal sealed class GetUsersQueryHandler(IApplicationDbContext context)
        : IRequestHandler<GetUsersQuery, List<UserResponse>>
    {
        public async Task<List<UserResponse>> Handle(
            GetUsersQuery request,
            CancellationToken cancellationToken)
        {
            var query = context.Users
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();

                query = query.Where(u =>
                    u.FirstName.ToLower().Contains(search) ||
                    u.LastName.ToLower().Contains(search) ||
                    u.Email.ToLower().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(request.Role)
                && Enum.TryParse<UserRole>(request.Role, true, out var role))
            {
                query = query.Where(u => u.Role == role);
            }

            return await query
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Select(u => new UserResponse
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Role = u.Role.ToString(),
                    Status = u.Status.ToString(),
                    DepartmentId = u.DepartmentId,
                    DepartmentName = u.Department != null ? u.Department.Name : null,
                    CreatedAtUtc = u.CreatedAtUtc
                })
                .ToListAsync(cancellationToken);
        }
    }
}
