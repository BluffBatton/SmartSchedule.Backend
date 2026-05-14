using MediatR;

namespace SmartSchedule.Application.Services.Bookings.GetStudentBookings
{
    public sealed record GetStudentBookingsQuery() : IRequest<List<StudentBookingResponse>>;
}