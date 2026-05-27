using MediatR;

namespace SmartSchedule.Application.Services.TeacherSlots.CreateSlot
{
    public sealed class CreateTeacherSlotCommand : IRequest<Guid>
    {
        public DateTime StartAtUtc { get; set; }

        public DateTime EndAtUtc { get; set; }
    }
}
