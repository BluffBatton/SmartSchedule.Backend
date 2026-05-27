namespace SmartSchedule.Infrastructure.BackgroundServices
{
    public sealed class InactiveUsersCleanupOptions
    {
        public const string SectionName = "InactiveUsersCleanup";

        public bool Enabled { get; set; } = true;

        public int InactivityThresholdDays { get; set; } = 180;

        public int RunIntervalHours { get; set; } = 24;

        public int InitialDelaySeconds { get; set; } = 30;

        public int BatchSize { get; set; } = 100;
    }
}
