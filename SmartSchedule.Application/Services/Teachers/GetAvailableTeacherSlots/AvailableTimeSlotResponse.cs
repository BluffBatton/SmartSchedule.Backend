namespace SmartSchedule.Application.Services.Teachers.GetAvailableTeacherSlots
{
    public sealed record AvailableTimeSlotResponse
    {
        public Guid Id { get; set; }

        public Guid TeacherId { get; set; }

        public DateTime StartAtUtc { get; set; }

        public DateTime EndAtUtc { get; set; }

        public required string Status { get; set; }
    }
}
