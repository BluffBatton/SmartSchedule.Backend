namespace SmartSchedule.Application.Common.Exceptions
{
    public sealed class UnauthorizedException : AppException
    {
        public override int StatusCode => 401;
        public override string Title => "Unauthorized";

        public UnauthorizedException(string message) : base(message) { }
    }
}
