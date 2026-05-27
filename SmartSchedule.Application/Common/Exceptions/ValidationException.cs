namespace SmartSchedule.Application.Common.Exceptions
{
    public sealed class ValidationException : AppException
    {
        public override int StatusCode => 400;
        public override string Title => "Validation failed";

        public ValidationException(string message) : base(message) { }
    }
}
