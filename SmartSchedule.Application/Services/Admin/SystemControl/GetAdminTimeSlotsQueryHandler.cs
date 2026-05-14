using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartSchedule.Application.Interfaces;
using SmartSchedule.Domain.Enums;

namespace SmartSchedule.Application.Services.Admin.SystemControl
{
    internal sealed class GetAdminTimeSlotsQueryHandler(IApplicationDbContext context)
        : IRequestHandler<GetAdminTimeSlotsQuery, List<AdminTimeSlotResponse>>
    {
        public async Task<List<AdminTimeSlotResponse>> Handle(
            GetAdminTimeSlotsQuery request,
            CancellationToken cancellationToken)
        {
            var query = context.TimeSlots
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Status)
                && Enum.TryParse<TimeSlotStatus>(request.Status, true, out var status))
            {
                query = query.Where(ts => ts.Status == status);
            }

            return await query
                .OrderByDescending(ts => ts.StartAtUtc)
                .Select(ts => new AdminTimeSlotResponse
                {
                    Id = ts.Id,
                    TeacherId = ts.TeacherId,
                    TeacherFullName = ts.Teacher.FirstName + " " + ts.Teacher.LastName,
                    DepartmentName = ts.Teacher.Department != null
                        ? ts.Teacher.Department.Name
                        : null,
                    StartAtUtc = ts.StartAtUtc,
                    EndAtUtc = ts.EndAtUtc,
                    Status = ts.Status.ToString()
                })
                .ToListAsync(cancellationToken);
        }
    }
}
