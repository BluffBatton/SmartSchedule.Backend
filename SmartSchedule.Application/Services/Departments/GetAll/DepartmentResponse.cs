namespace SmartSchedule.Application.Services.Departments.GetAll
{
    public sealed record DepartmentResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int TeachersCount { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }
}
