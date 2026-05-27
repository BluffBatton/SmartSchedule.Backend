namespace SmartSchedule.Application.Services.Notifications.GetMyNotifications
{
    public sealed record NotificationResponse
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public bool IsRead { get; set; }

        public DateTime? ReadAtUtc { get; set; }

        public DateTime? SentAtUtc { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public Guid? BookingId { get; set; }
    }
}
