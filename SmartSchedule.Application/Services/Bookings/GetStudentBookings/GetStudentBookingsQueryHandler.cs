using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartSchedule.Application.Common.Exceptions;
using SmartSchedule.Application.Interfaces;

namespace SmartSchedule.Application.Services.Bookings.GetStudentBookings
{
    internal sealed class GetStudentBookingsQueryHandler(
        IApplicationDbContext context,
        IUserContextService userContextService)
        : IRequestHandler<GetStudentBookingsQuery, List<StudentBookingResponse>>
    {
        public async Task<List<StudentBookingResponse>> Handle(
            GetStudentBookingsQuery request,
            CancellationToken cancellationToken)
        {
            var studentId = userContextService.GetCurrentUserId();

            if (studentId is null)
            {
                throw new UnauthorizedException("User is not authenticated.");
            }

            return await context.Bookings
                .AsNoTracking()
                .Where(b => b.StudentId == studentId.Value)
                .OrderByDescending(b => b.BookedAtUtc)
                .Select(b => new StudentBookingResponse
                {
                    Id = b.Id,
                    TimeSlotId = b.TimeSlotId,
                    TeacherId = b.TimeSlot.TeacherId,
                    TeacherFullName = b.TimeSlot.Teacher.FirstName + " " + b.TimeSlot.Teacher.LastName,
                    DepartmentName = b.TimeSlot.Teacher.Department != null
                        ? b.TimeSlot.Teacher.Department.Name
                        : null,
                    StartAtUtc = b.TimeSlot.StartAtUtc,
                    EndAtUtc = b.TimeSlot.EndAtUtc,
                    Status = b.Status.ToString(),
                    BookedAtUtc = b.BookedAtUtc,
                    CancelledAtUtc = b.CancelledAtUtc
                })
                .ToListAsync(cancellationToken);
        }
    }
}
