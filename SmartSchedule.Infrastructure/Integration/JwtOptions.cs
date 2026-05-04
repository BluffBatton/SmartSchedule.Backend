namespace SmartSchedule.Infrastructure.Integration
{
    public class JwtOptions
    {
        public const string SectionName = "Jwt";

        public required string Issuer { get; init; }
        public required string Audience { get; init; }
        public required string Key { get; init; }
        public int ExpirationMinutes { get; init; }
    }
}
