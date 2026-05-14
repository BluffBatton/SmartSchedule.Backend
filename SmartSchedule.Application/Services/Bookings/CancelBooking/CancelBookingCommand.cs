using MediatR;

namespace SmartSchedule.Application.Services.Bookings.CancelBooking
{
    public sealed record CancelBookingCommand(
        Guid BookingId,
        string? CancelReason
    ) : IRequest;
}