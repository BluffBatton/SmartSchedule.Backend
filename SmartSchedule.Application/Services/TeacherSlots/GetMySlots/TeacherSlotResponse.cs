namespace SmartSchedule.Application.Services.TeacherSlots.GetMySlots
{
    public sealed record TeacherSlotResponse
    {
        public Guid Id { get; set; }

        public DateTime StartAtUtc { get; set; }

        public DateTime EndAtUtc { get; set; }

        public string Status { get; set; } = string.Empty;

        public bool HasActiveBooking { get; set; }

        public Guid? StudentId { get; set; }

        public string? StudentFullName { get; set; }
    }
}
