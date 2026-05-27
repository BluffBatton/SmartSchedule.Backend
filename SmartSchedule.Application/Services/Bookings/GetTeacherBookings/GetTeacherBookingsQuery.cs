using MediatR;

namespace SmartSchedule.Application.Services.Bookings.GetTeacherBookings
{
    public sealed record GetTeacherBookingsQuery(
        string? Status,
        string? Scope
    ) : IRequest<List<TeacherBookingResponse>>;
}
