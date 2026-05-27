using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartSchedule.Application.Common.Exceptions;
using SmartSchedule.Application.Interfaces;
using SmartSchedule.Domain.Enums;

namespace SmartSchedule.Application.Services.Bookings.DeleteBooking
{
    internal sealed class DeleteBookingCommandHandler(IApplicationDbContext context)
        : IRequestHandler<DeleteBookingCommand>
    {
        public async Task Handle(
            DeleteBookingCommand request,
            CancellationToken cancellationToken)
        {
            var booking = await context.Bookings
                .FirstOrDefaultAsync(b => b.Id == request.BookingId, cancellationToken);

            if (booking is null)
            {
                throw new NotFoundException(
                    $"Booking with id '{request.BookingId}' was not found.");
            }

            if (booking.Status == BookingStatus.Active)
            {
                throw new ConflictException(
                    "Active booking cannot be deleted. Cancel it first.");
            }

            context.Bookings.Remove(booking);

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
