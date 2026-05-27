using Microsoft.EntityFrameworkCore;
using SmartSchedule.Application.Common.Exceptions;
using SmartSchedule.Application.Interfaces;
using SmartSchedule.Domain.Enums;

namespace SmartSchedule.Application.Services.TeacherSlots.Common
{
    internal static class TeacherSlotValidator
    {
        public static readonly TimeSpan MinDuration = TimeSpan.FromMinutes(15);
        public static readonly TimeSpan MaxDuration = TimeSpan.FromHours(8);

        public static void ValidateRange(DateTime startAtUtc, DateTime endAtUtc, DateTime now)
        {
            if (startAtUtc.Kind == DateTimeKind.Unspecified)
                startAtUtc = DateTime.SpecifyKind(startAtUtc, DateTimeKind.Utc);

            if (endAtUtc.Kind == DateTimeKind.Unspecified)
                endAtUtc = DateTime.SpecifyKind(endAtUtc, DateTimeKind.Utc);

            if (startAtUtc >= endAtUtc)
                throw new ValidationException("Slot start time must be earlier than its end time.");

            if (startAtUtc <= now)
                throw new ValidationException("Slot start time must be in the future.");

            var duration = endAtUtc - startAtUtc;

            if (duration < MinDuration)
                throw new ValidationException(
                    $"Slot duration must be at least {(int)MinDuration.TotalMinutes} minutes.");

            if (duration > MaxDuration)
                throw new ValidationException(
                    $"Slot duration must not exceed {(int)MaxDuration.TotalHours} hours.");
        }

        public static async Task EnsureNoOverlapAsync(
            IApplicationDbContext context,
            Guid teacherId,
            DateTime startAtUtc,
            DateTime endAtUtc,
            Guid? excludeSlotId,
            CancellationToken cancellationToken)
        {
            var overlap = await context.TimeSlots
                .AnyAsync(ts =>
                    ts.TeacherId == teacherId &&
                    ts.Status != TimeSlotStatus.Cancelled &&
                    (excludeSlotId == null || ts.Id != excludeSlotId) &&
                    ts.StartAtUtc < endAtUtc &&
                    ts.EndAtUtc > startAtUtc,
                    cancellationToken);

            if (overlap)
            {
                throw new ConflictException(
                    "Slot overlaps with an existing time slot of this teacher.");
            }
        }
    }
}
