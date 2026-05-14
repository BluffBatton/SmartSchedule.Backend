using MediatR;

namespace SmartSchedule.Application.Services.Teachers.GetAvailableTeacherSlots
{
    public sealed record GetAvailableTeacherSlotsQuery(Guid TeacherId)
        : IRequest<List<AvailableTimeSlotResponse>>;
}