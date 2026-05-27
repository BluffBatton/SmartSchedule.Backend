using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartSchedule.Application.Common.Exceptions;
using SmartSchedule.Application.Interfaces;
using SmartSchedule.Domain.Enums;

namespace SmartSchedule.Application.Services.Admin.Teachers.GetAdminTeachers
{
    internal sealed class GetAdminTeachersQueryHandler(IApplicationDbContext context)
        : IRequestHandler<GetAdminTeachersQuery, List<AdminTeacherResponse>>
    {
        public async Task<List<AdminTeacherResponse>> Handle(
            GetAdminTeachersQuery request,
            CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            var query = context.Users
                .AsNoTracking()
                .Where(u => u.Role == UserRole.Teacher);

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

            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                if (!Enum.TryParse<UserStatus>(request.Status, true, out var status))
                {
                    throw new ValidationException($"Invalid status '{request.Status}'.");
                }

                query = query.Where(u => u.Status == status);
            }

            return await query
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Select(u => new AdminTeacherResponse
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Status = u.Status.ToString(),
                    DepartmentId = u.DepartmentId,
                    DepartmentName = u.Department != null ? u.Department.Name : null,
                    TotalSlotsCount = u.TeacherTimeSlots.Count(),
                    AvailableSlotsCount = u.TeacherTimeSlots.Count(ts =>
                        ts.Status == TimeSlotStatus.Available &&
                        ts.StartAtUtc > now),
                    ActiveBookingsCount = u.TeacherTimeSlots.Count(ts =>
                        ts.Booking != null &&
                        ts.Booking.Status == BookingStatus.Active &&
                        ts.StartAtUtc > now),
                    LastLoginAtUtc = u.LastLoginAtUtc,
                    CreatedAtUtc = u.CreatedAtUtc
                })
                .ToListAsync(cancellationToken);
        }
    }
}
