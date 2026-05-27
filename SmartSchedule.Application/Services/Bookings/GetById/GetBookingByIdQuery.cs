using MediatR;

namespace SmartSchedule.Application.Services.Bookings.GetById
{
    public sealed record GetBookingByIdQuery(Guid BookingId)
        : IRequest<BookingDetailsResponse>;
}
