using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartSchedule.Application.Interfaces;
using SmartSchedule.Domain.Enums;

namespace SmartSchedule.Application.Services.Students.Dashboard
{
    internal sealed class GetStudentDashboardQueryHandler(
        IApplicationDbContext context,
        IUserContextService userContextService)
        : IRequestHandler<GetStudentDashboardQuery, DashboardResponse>
    {
        public async Task<DashboardResponse> Handle(
            GetStudentDashboardQuery request,
            CancellationToken cancellationToken)
        {
            var userId = userContextService.GetCurrentUserId();

            if (userId is null)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var now = DateTime.UtcNow;

            var activeBookings = await context.Bookings
                .CountAsync(b =>
                    b.StudentId == userId.Value &&
                    b.Status == BookingStatus.Active,
                    cancellationToken);

            var availableTeachers = await context.Users
                .CountAsync(u =>
                    u.Role == UserRole.Teacher &&
                    u.Status == UserStatus.Active &&
                    u.TeacherTimeSlots.Any(ts =>
                        ts.Status == TimeSlotStatus.Available &&
                        ts.StartAtUtc > now),
                    cancellationToken);

            var upcomingMeetings = await context.Bookings
                .CountAsync(b =>
                    b.StudentId == userId.Value &&
                    b.Status == BookingStatus.Active &&
                    b.TimeSlot.StartAtUtc > now,
                    cancellationToken);

            var consultations = await context.Bookings
                .Where(b =>
                    b.StudentId == userId.Value &&
                    b.Status == BookingStatus.Active &&
                    b.TimeSlot.StartAtUtc > now)
                .OrderBy(b => b.TimeSlot.StartAtUtc)
                .Take(5)
                .Select(b => new ConsultationResponse
                {
                    TeacherFullName = b.TimeSlot.Teacher.FirstName + " " + b.TimeSlot.Teacher.LastName,
                    DepartmentName = b.TimeSlot.Teacher.Department != null
                        ? b.TimeSlot.Teacher.Department.Name
                        : "-",
                    ConsultationStartTime = b.TimeSlot.StartAtUtc,
                    ConsultationEndTime = b.TimeSlot.EndAtUtc,
                    BookingDate = b.TimeSlot.StartAtUtc.Date
                })
                .ToListAsync(cancellationToken);

            return new DashboardResponse
            {
                ActiveBookings = activeBookings,
                AvailableTeachers = availableTeachers,
                UpcomingMeetings = upcomingMeetings,
                Consultations = consultations
            };
        }
    }
}