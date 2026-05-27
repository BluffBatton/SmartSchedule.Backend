using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartSchedule.Application.Common.Exceptions;
using SmartSchedule.Application.Interfaces;
using SmartSchedule.Domain.Enums;

namespace SmartSchedule.Application.Services.TeacherSlots.DeleteSlot
{
    internal sealed class DeleteTeacherSlotCommandHandler(
        IApplicationDbContext context,
        IUserContextService userContextService)
        : IRequestHandler<DeleteTeacherSlotCommand>
    {
        public async Task Handle(
            DeleteTeacherSlotCommand request,
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
                throw new ForbiddenException("You can only delete your own slots.");
            }

            var hasActiveBooking = await context.Bookings
                .AnyAsync(b =>
                    b.TimeSlotId == slot.Id &&
                    b.Status == BookingStatus.Active,
                    cancellationToken);

            if (hasActiveBooking)
            {
                throw new ConflictException(
                    "Slot has an active booking and cannot be deleted. Cancel the booking first.");
            }

            context.TimeSlots.Remove(slot);

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
