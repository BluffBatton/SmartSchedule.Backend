namespace SmartSchedule.Application.Services.Bookings.GetById
{
    public sealed record BookingDetailsResponse
    {
        public Guid Id { get; set; }

        public Guid TimeSlotId { get; set; }

        public Guid StudentId { get; set; }

        public string StudentFullName { get; set; } = string.Empty;

        public string StudentEmail { get; set; } = string.Empty;

        public Guid TeacherId { get; set; }

        public string TeacherFullName { get; set; } = string.Empty;

        public string TeacherEmail { get; set; } = string.Empty;

        public string? DepartmentName { get; set; }

        public DateTime StartAtUtc { get; set; }

        public DateTime EndAtUtc { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime BookedAtUtc { get; set; }

        public DateTime? CancelledAtUtc { get; set; }

        public string? CancelReason { get; set; }
    }
}
