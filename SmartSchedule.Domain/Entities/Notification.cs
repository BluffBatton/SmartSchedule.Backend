using SmartSchedule.Domain.Common;
using SmartSchedule.Domain.Enums;

namespace SmartSchedule.Domain.Entities
{
    public class Notification : BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public Guid? BookingId { get; set; }
        public Booking? Booking { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public NotificationType Type { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime? ReadAtUtc { get; set; }

        public DateTime? ScheduledAtUtc { get; set; }

        public DateTime? SentAtUtc { get; set; }
    }
}
