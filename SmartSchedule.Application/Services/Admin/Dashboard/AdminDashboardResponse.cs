namespace SmartSchedule.Application.Services.Admin.Dashboard
{
    public sealed record AdminDashboardResponse
    {
        public int TotalUsers { get; set; }
        public int StudentsCount { get; set; }
        public int TeachersCount { get; set; }
        public int ActiveBookingsCount { get; set; }

        public int TotalTimeSlots { get; set; }
        public int AvailableSlots { get; set; }
        public int BookedSlots { get; set; }
        public int CancelledBookings { get; set; }

        public List<RecentBookingResponse> RecentBookings { get; set; } = new();
    }

    public sealed record RecentBookingResponse
    {
        public Guid BookingId { get; set; }
        public string StudentFullName { get; set; } = string.Empty;
        public string TeacherFullName { get; set; } = string.Empty;
        public DateTime StartAtUtc { get; set; }
        public DateTime EndAtUtc { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
