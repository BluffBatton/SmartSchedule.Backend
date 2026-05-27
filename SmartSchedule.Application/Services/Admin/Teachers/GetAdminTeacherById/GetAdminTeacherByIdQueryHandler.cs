using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartSchedule.Application.Common.Exceptions;
using SmartSchedule.Application.Interfaces;
using SmartSchedule.Domain.Enums;

namespace SmartSchedule.Application.Services.Admin.Teachers.GetAdminTeacherById
{
    internal sealed class GetAdminTeacherByIdQueryHandler(IApplicationDbContext context)
        : IRequestHandler<GetAdminTeacherByIdQuery, AdminTeacherDetailsResponse>
    {
        public async Task<AdminTeacherDetailsResponse> Handle(
            GetAdminTeacherByIdQuery request,
            CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            var teacher = await context.Users
                .AsNoTracking()
                .Where(u => u.Id == request.TeacherId && u.Role == UserRole.Teacher)
                .Select(u => new AdminTeacherDetailsResponse
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Status = u.Status.ToString(),
                    DepartmentId = u.DepartmentId,
                    DepartmentName = u.Department != null ? u.Department.Name : null,
                    CreatedByAdminId = u.CreatedByAdminId,
                    TotalSlotsCount = u.TeacherTimeSlots.Count(),
                    AvailableSlotsCount = u.TeacherTimeSlots.Count(ts =>
                        ts.Status == TimeSlotStatus.Available &&
                        ts.StartAtUtc > now),
                    FutureBookedSlotsCount = u.TeacherTimeSlots.Count(ts =>
                        ts.Status == TimeSlotStatus.Booked &&
                        ts.StartAtUtc > now),
                    ActiveBookingsCount = u.TeacherTimeSlots.Count(ts =>
                        ts.Booking != null &&
                        ts.Booking.Status == BookingStatus.Active &&
                        ts.StartAtUtc > now),
                    LastLoginAtUtc = u.LastLoginAtUtc,
                    CreatedAtUtc = u.CreatedAtUtc,
                    UpdatedAtUtc = u.UpdatedAtUtc
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (teacher is null)
            {
                throw new NotFoundException($"Teacher with id '{request.TeacherId}' was not found.");
            }

            return teacher;
        }
    }
}
