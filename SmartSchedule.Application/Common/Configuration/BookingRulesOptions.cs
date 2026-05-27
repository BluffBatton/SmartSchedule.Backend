namespace SmartSchedule.Application.Common.Configuration
{
    public sealed class BookingRulesOptions
    {
        public const string SectionName = "BookingRules";

        public int CancelDeadlineHours { get; set; } = 2;

        public int MaxActiveBookingsPerStudent { get; set; } = 3;
    }
}
