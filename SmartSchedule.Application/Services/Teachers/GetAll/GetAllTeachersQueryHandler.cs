using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartSchedule.Application.Interfaces;
using SmartSchedule.Domain.Enums;

namespace SmartSchedule.Application.Services.Teachers.GetAll
{
    internal sealed class GetTeachersQueryHandler(IApplicationDbContext context)
        : IRequestHandler<GetTeachersQuery, List<TeacherResponse>>
    {
        public async Task<List<TeacherResponse>> Handle(
            GetTeachersQuery request,
            CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            var query = context.Users
                .AsNoTracking()
                .Where(u =>
                    u.Role == UserRole.Teacher &&
                    u.Status == UserStatus.Active);

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();

                query = query.Where(u =>
                    u.FirstName.ToLower().Contains(search) ||
                    u.LastName.ToLower().Contains(search) ||
                    u.Email.ToLower().Contains(search) ||
                    (u.Department != null && u.Department.Name.ToLower().Contains(search)));
            }

            if (request.DepartmentId.HasValue)
            {
                query = query.Where(u => u.DepartmentId == request.DepartmentId.Value);
            }

            return await query
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Select(u => new TeacherResponse
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    DepartmentId = u.DepartmentId,
                    DepartmentName = u.Department != null ? u.Department.Name : null,
                    AvailableSlotsCount = u.TeacherTimeSlots.Count(ts =>
                        ts.Status == TimeSlotStatus.Available &&
                        ts.StartAtUtc > now)
                })
                .ToListAsync(cancellationToken);
        }
    }
}
