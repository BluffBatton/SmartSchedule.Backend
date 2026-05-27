using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartSchedule.Application.Common.Exceptions;
using SmartSchedule.Application.Interfaces;
using SmartSchedule.Domain.Enums;

namespace SmartSchedule.Application.Services.TeacherSlots.GetMySlots
{
    internal sealed class GetMyTeacherSlotsQueryHandler(
        IApplicationDbContext context,
        IUserContextService userContextService)
        : IRequestHandler<GetMyTeacherSlotsQuery, List<TeacherSlotResponse>>
    {
        public async Task<List<TeacherSlotResponse>> Handle(
            GetMyTeacherSlotsQuery request,
            CancellationToken cancellationToken)
        {
            var teacherId = userContextService.GetCurrentUserId()
                ?? throw new UnauthorizedException("User is not authenticated.");

            var now = DateTime.UtcNow;

            var query = context.TimeSlots
                .AsNoTracking()
                .Where(ts => ts.TeacherId == teacherId);

            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                if (!Enum.TryParse<TimeSlotStatus>(request.Status, true, out var status))
                {
                    throw new ValidationException($"Invalid status '{request.Status}'.");
                }

                query = query.Where(ts => ts.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(request.Scope))
            {
                var scope = request.Scope.Trim().ToLowerInvariant();
                query = scope switch
                {
                    "upcoming" => query.Where(ts => ts.StartAtUtc > now),
                    "past" => query.Where(ts => ts.EndAtUtc <= now),
                    "all" => query,
                    _ => throw new ValidationException(
                        $"Invalid scope '{request.Scope}'. Allowed: upcoming, past, all.")
                };
            }

            return await query
                .OrderBy(ts => ts.StartAtUtc)
                .Select(ts => new TeacherSlotResponse
                {
                    Id = ts.Id,
                    StartAtUtc = ts.StartAtUtc,
                    EndAtUtc = ts.EndAtUtc,
                    Status = ts.Status.ToString(),
                    HasActiveBooking = ts.Booking != null && ts.Booking.Status == BookingStatus.Active,
                    StudentId = ts.Booking != null && ts.Booking.Status == BookingStatus.Active
                        ? ts.Booking.StudentId
                        : (Guid?)null,
                    StudentFullName = ts.Booking != null && ts.Booking.Status == BookingStatus.Active
                        ? ts.Booking.Student.FirstName + " " + ts.Booking.Student.LastName
                        : null
                })
                .ToListAsync(cancellationToken);
        }
    }
}
