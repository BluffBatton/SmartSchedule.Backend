namespace SmartSchedule.Application.Services.Bookings.GetStudentBookings
{
    public sealed record StudentBookingResponse
    {
        public Guid Id { get; set; }

        public Guid TimeSlotId { get; set; }

        public Guid TeacherId { get; set; }

        public required string TeacherFullName { get; set; }

        public string? DepartmentName { get; set; }

        public DateTime StartAtUtc { get; set; }

        public DateTime EndAtUtc { get; set; }

        public required string Status { get; set; }

        public DateTime BookedAtUtc { get; set; }

        public DateTime? CancelledAtUtc { get; set; }
    }
}
