namespace SmartSchedule.Application.Common.Exceptions
{
    public abstract class AppException : Exception
    {
        public abstract int StatusCode { get; }

        public virtual string Title => "Application error";

        protected AppException(string message) : base(message) { }
    }
}
