using MediatR;

namespace SmartSchedule.Application.Services.TeacherSlots.DeleteSlot
{
    public sealed record DeleteTeacherSlotCommand(Guid SlotId) : IRequest;
}
