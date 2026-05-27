namespace SmartSchedule.Application.Services.Admin.Teachers.GetAdminTeachers
{
    public sealed record AdminTeacherResponse
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string FullName => $"{FirstName} {LastName}";

        public string Email { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public Guid? DepartmentId { get; set; }

        public string? DepartmentName { get; set; }

        public int TotalSlotsCount { get; set; }

        public int AvailableSlotsCount { get; set; }

        public int ActiveBookingsCount { get; set; }

        public DateTime? LastLoginAtUtc { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }
}
