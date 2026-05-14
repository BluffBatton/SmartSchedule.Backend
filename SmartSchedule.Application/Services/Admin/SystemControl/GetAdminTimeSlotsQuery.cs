using MediatR;

namespace SmartSchedule.Application.Services.Admin.SystemControl
{
    public sealed record GetAdminTimeSlotsQuery(string? Status)
        : IRequest<List<AdminTimeSlotResponse>>;
}
