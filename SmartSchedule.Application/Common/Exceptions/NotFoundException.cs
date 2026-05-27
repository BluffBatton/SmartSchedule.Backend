namespace SmartSchedule.Application.Common.Exceptions
{
    public sealed class NotFoundException : AppException
    {
        public override int StatusCode => 404;
        public override string Title => "Resource not found";

        public NotFoundException(string message) : base(message) { }

        public static NotFoundException For(string entity, object key) =>
            new($"{entity} with id '{key}' was not found.");
    }
}
