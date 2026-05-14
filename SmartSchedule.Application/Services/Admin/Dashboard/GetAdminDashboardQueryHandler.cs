using MediatR;
using SmartSchedule.Application.Interfaces;
using SmartSchedule.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace SmartSchedule.Application.Services.Admin.Dashboard
{
    internal sealed class GetAdminDashboardQueryHandler(IApplicationDbContext context)
        : IRequestHandler<GetAdminDashboardQuery, AdminDashboardResponse>
    {
        public async Task<AdminDashboardResponse> Handle(
            GetAdminDashboardQuery request,
            CancellationToken cancellationToken)
        {
            var totalUsers = await context.Users
                .CountAsync(cancellationToken);

            var studentsCount = await context.Users
                .CountAsync(u => u.Role == UserRole.Student, cancellationToken);

            var teachersCount = await context.Users
                .CountAsync(u => u.Role == UserRole.Teacher, cancellationToken);

            var activeBookingsCount = await context.Bookings
                .CountAsync(b => b.Status == BookingStatus.Active, cancellationToken);

            var totalTimeSlots = await context.TimeSlots
                .CountAsync(cancellationToken);

            var availableSlots = await context.TimeSlots
                .CountAsync(ts => ts.Status == TimeSlotStatus.Available, cancellationToken);

            var bookedSlots = await context.TimeSlots
                .CountAsync(ts => ts.Status == TimeSlotStatus.Booked, cancellationToken);

            var cancelledBookings = await context.Bookings
                .CountAsync(b => b.Status == BookingStatus.Cancelled, cancellationToken);

            var recentBookings = await context.Bookings
                .AsNoTracking()
                .OrderByDescending(b => b.BookedAtUtc)
                .Take(5)
                .Select(b => new RecentBookingResponse
                {
                    BookingId = b.Id,
                    StudentFullName = b.Student.FirstName + " " + b.Student.LastName,
                    TeacherFullName = b.TimeSlot.Teacher.FirstName + " " + b.TimeSlot.Teacher.LastName,
                    StartAtUtc = b.TimeSlot.StartAtUtc,
                    EndAtUtc = b.TimeSlot.EndAtUtc,
                    Status = b.Status.ToString()
                })
                .ToListAsync(cancellationToken);

            return new AdminDashboardResponse
            {
                TotalUsers = totalUsers,
                StudentsCount = studentsCount,
                TeachersCount = teachersCount,
                ActiveBookingsCount = activeBookingsCount,
                TotalTimeSlots = totalTimeSlots,
                AvailableSlots = availableSlots,
                BookedSlots = bookedSlots,
                CancelledBookings = cancelledBookings,
                RecentBookings = recentBookings
            };
        }
    }
}
