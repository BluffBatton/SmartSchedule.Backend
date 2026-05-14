namespace SmartSchedule.Application.Services.Admin.SystemControl
{
    public sealed record AdminBookingResponse
    {
        public Guid Id { get; set; }

        public string StudentFullName { get; set; } = string.Empty;

        public string TeacherFullName { get; set; } = string.Empty;

        public string? DepartmentName { get; set; }

        public DateTime StartAtUtc { get; set; }

        public DateTime EndAtUtc { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime BookedAtUtc { get; set; }
    }
}
