using MediatR;

namespace SmartSchedule.Application.Services.TeacherSlots.UpdateSlot
{
    public sealed class UpdateTeacherSlotCommand : IRequest
    {
        public Guid SlotId { get; set; }

        public DateTime StartAtUtc { get; set; }

        public DateTime EndAtUtc { get; set; }
    }
}
