using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartSchedule.Application.Common.Exceptions;
using SmartSchedule.Application.Interfaces;
using SmartSchedule.Domain.Enums;

namespace SmartSchedule.Application.Services.Teachers.GetAvailableTeacherSlots
{
    internal sealed class GetAvailableTeacherSlotsQueryHandler(IApplicationDbContext context)
        : IRequestHandler<GetAvailableTeacherSlotsQuery, List<AvailableTimeSlotResponse>>
    {
        public async Task<List<AvailableTimeSlotResponse>> Handle(
            GetAvailableTeacherSlotsQuery request,
            CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            var teacherExists = await context.Users
                .AnyAsync(u =>
                    u.Id == request.TeacherId &&
                    u.Role == UserRole.Teacher &&
                    u.Status == UserStatus.Active,
                    cancellationToken);

            if (!teacherExists)
            {
                throw new NotFoundException("Teacher not found.");
            }

            return await context.TimeSlots
                .AsNoTracking()
                .Where(ts =>
                    ts.TeacherId == request.TeacherId &&
                    ts.Status == TimeSlotStatus.Available &&
                    ts.StartAtUtc > now)
                .OrderBy(ts => ts.StartAtUtc)
                .Select(ts => new AvailableTimeSlotResponse
                {
                    Id = ts.Id,
                    TeacherId = ts.TeacherId,
                    StartAtUtc = ts.StartAtUtc,
                    EndAtUtc = ts.EndAtUtc,
                    Status = ts.Status.ToString()
                })
                .ToListAsync(cancellationToken);
        }
    }
}
