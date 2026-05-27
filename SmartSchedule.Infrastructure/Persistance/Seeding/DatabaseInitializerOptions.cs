namespace SmartSchedule.Infrastructure.Persistance.Seeding
{
    public sealed class DatabaseInitializerOptions
    {
        public const string SectionName = "DatabaseInitializer";

        public bool ApplyMigrationsOnStartup { get; set; } = true;

        public SeedAdminOptions SeedAdmin { get; set; } = new();
    }

    public sealed class SeedAdminOptions
    {
        public bool Enabled { get; set; } = true;

        public string Email { get; set; } = "admin@smartschedule.local";

        public string Password { get; set; } = "ChangeMe123!";

        public string FirstName { get; set; } = "System";

        public string LastName { get; set; } = "Admin";
    }
}
