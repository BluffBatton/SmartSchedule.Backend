using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartSchedule.Application.Common.Exceptions;
using SmartSchedule.Application.Interfaces;
using SmartSchedule.Application.Services.TeacherSlots.Common;
using SmartSchedule.Domain.Enums;

namespace SmartSchedule.Application.Services.TeacherSlots.UpdateSlot
{
    internal sealed class UpdateTeacherSlotCommandHandler(
        IApplicationDbContext context,
        IUserContextService userContextService)
        : IRequestHandler<UpdateTeacherSlotCommand>
    {
        public async Task Handle(
            UpdateTeacherSlotCommand request,
            CancellationToken cancellationToken)
        {
            var teacherId = userContextService.GetCurrentUserId()
                ?? throw new UnauthorizedException("User is not authenticated.");

            var slot = await context.TimeSlots
                .FirstOrDefaultAsync(ts => ts.Id == request.SlotId, cancellationToken);

            if (slot is null)
            {
                throw new NotFoundException($"Time slot with id '{request.SlotId}' was not found.");
            }

            if (slot.TeacherId != teacherId)
            {
                throw new ForbiddenException("You can only modify your own slots.");
            }

            if (slot.Status != TimeSlotStatus.Available)
            {
                throw new ConflictException(
                    $"Slot cannot be modified in status '{slot.Status}'. " +
                    "Only available slots can be edited.");
            }

            var hasActiveBooking = await context.Bookings
                .AnyAsync(b =>
                    b.TimeSlotId == slot.Id &&
                    b.Status == BookingStatus.Active,
                    cancellationToken);

            if (hasActiveBooking)
            {
                throw new ConflictException(
                    "Slot has an active booking and cannot be modified. Cancel the booking first.");
            }

            var startUtc = DateTime.SpecifyKind(request.StartAtUtc, DateTimeKind.Utc);
            var endUtc = DateTime.SpecifyKind(request.EndAtUtc, DateTimeKind.Utc);

            TeacherSlotValidator.ValidateRange(startUtc, endUtc, DateTime.UtcNow);

            await TeacherSlotValidator.EnsureNoOverlapAsync(
                context,
                teacherId,
                startUtc,
                endUtc,
                excludeSlotId: slot.Id,
                cancellationToken);

            slot.StartAtUtc = startUtc;
            slot.EndAtUtc = endUtc;

            try
            {
                await context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException)
            {
                throw new ConflictException(
                    "Slot with the same start and end time already exists for this teacher.");
            }
        }
    }
}
