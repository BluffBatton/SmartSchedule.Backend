using SmartSchedule.Domain.Common;
using SmartSchedule.Domain.Enums;

namespace SmartSchedule.Domain.Entities
{
    public class TimeSlot : BaseEntity
    {
        public Guid TeacherId { get; set; }
        public User Teacher { get; set; } = null!;

        public DateTime StartAtUtc { get; set; }

        public DateTime EndAtUtc { get; set; }

        public TimeSlotStatus Status { get; set; } = TimeSlotStatus.Available;

        public Booking? Booking { get; set; }
    }
}
