namespace SmartSchedule.Application.Services.Bookings.GetTeacherBookings
{
    public sealed record TeacherBookingResponse
    {
        public Guid Id { get; set; }

        public Guid TimeSlotId { get; set; }

        public Guid StudentId { get; set; }

        public string StudentFullName { get; set; } = string.Empty;

        public string StudentEmail { get; set; } = string.Empty;

        public DateTime StartAtUtc { get; set; }

        public DateTime EndAtUtc { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime BookedAtUtc { get; set; }

        public DateTime? CancelledAtUtc { get; set; }
    }
}
