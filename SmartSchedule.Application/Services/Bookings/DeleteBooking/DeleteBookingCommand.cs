using MediatR;

namespace SmartSchedule.Application.Services.Bookings.DeleteBooking
{
    public sealed record DeleteBookingCommand(Guid BookingId) : IRequest;
}
