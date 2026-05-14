using MediatR;

namespace SmartSchedule.Application.Services.Bookings.Create
{
    public sealed class CreateBookingCommand : IRequest<Guid>
    {
        public Guid TimeSlotId { get; set; }
    }
}
