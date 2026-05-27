namespace SmartSchedule.Application.Common.Exceptions
{
    public sealed class ForbiddenException : AppException
    {
        public override int StatusCode => 403;
        public override string Title => "Forbidden";

        public ForbiddenException(string message) : base(message) { }
    }
}
