using MediatR;

namespace SmartSchedule.Application.Services.TeacherSlots.GetMySlots
{
    public sealed record GetMyTeacherSlotsQuery(
        string? Status,
        string? Scope
    ) : IRequest<List<TeacherSlotResponse>>;
}
