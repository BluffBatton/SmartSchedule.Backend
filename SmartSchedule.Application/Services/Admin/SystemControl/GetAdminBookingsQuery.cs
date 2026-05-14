using MediatR;

namespace SmartSchedule.Application.Services.Admin.SystemControl
{
    public sealed record GetAdminBookingsQuery(string? Status)
        : IRequest<List<AdminBookingResponse>>;
}
