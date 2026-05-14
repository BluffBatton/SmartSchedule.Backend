namespace SmartSchedule.Application.Services.Teachers.GetById
{
    public sealed record TeacherDetailsResponse
    {
        public Guid Id { get; set; }

        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public required string Email { get; set; }

        public Guid? DepartmentId { get; set; }

        public string? DepartmentName { get; set; }

        public int AvailableSlotsCount { get; set; }
    }
}
