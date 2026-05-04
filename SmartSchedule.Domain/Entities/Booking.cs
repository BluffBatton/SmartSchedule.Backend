using SmartSchedule.Domain.Common;
using SmartSchedule.Domain.Enums;

namespace SmartSchedule.Domain.Entities
{
    public class Booking : BaseEntity
    {
        public Guid StudentId { get; set; }
        public User Student { get; set; } = null!;

        public Guid TimeSlotId { get; set; }
        public TimeSlot TimeSlot { get; set; } = null!;

        public BookingStatus Status { get; set; } = BookingStatus.Active;

        public DateTime BookedAtUtc { get; set; }

        public DateTime? CancelledAtUtc { get; set; }

        public string? CancelReason { get; set; }

        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
