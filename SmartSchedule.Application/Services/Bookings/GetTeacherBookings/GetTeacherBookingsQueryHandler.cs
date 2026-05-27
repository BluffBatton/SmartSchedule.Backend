using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartSchedule.Application.Common.Exceptions;
using SmartSchedule.Application.Interfaces;
using SmartSchedule.Domain.Enums;

namespace SmartSchedule.Application.Services.Bookings.GetTeacherBookings
{
    internal sealed class GetTeacherBookingsQueryHandler(
        IApplicationDbContext context,
        IUserContextService userContextService)
        : IRequestHandler<GetTeacherBookingsQuery, List<TeacherBookingResponse>>
    {
        public async Task<List<TeacherBookingResponse>> Handle(
            GetTeacherBookingsQuery request,
            CancellationToken cancellationToken)
        {
            var teacherId = userContextService.GetCurrentUserId()
                ?? throw new UnauthorizedException("User is not authenticated.");

            var now = DateTime.UtcNow;

            var query = context.Bookings
                .AsNoTracking()
                .Where(b => b.TimeSlot.TeacherId == teacherId);

            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                if (!Enum.TryParse<BookingStatus>(request.Status, true, out var status))
                {
                    throw new ValidationException($"Invalid status '{request.Status}'.");
                }

                query = query.Where(b => b.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(request.Scope))
            {
                var scope = request.Scope.Trim().ToLowerInvariant();
                query = scope switch
                {
                    "upcoming" => query.Where(b => b.TimeSlot.StartAtUtc > now),
                    "past" => query.Where(b => b.TimeSlot.EndAtUtc <= now),
                    "all" => query,
                    _ => throw new ValidationException(
                        $"Invalid scope '{request.Scope}'. Allowed: upcoming, past, all.")
                };
            }

            return await query
                .OrderByDescending(b => b.TimeSlot.StartAtUtc)
                .Select(b => new TeacherBookingResponse
                {
                    Id = b.Id,
                    TimeSlotId = b.TimeSlotId,
                    StudentId = b.StudentId,
                    StudentFullName = b.Student.FirstName + " " + b.Student.LastName,
                    StudentEmail = b.Student.Email,
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
