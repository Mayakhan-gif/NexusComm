namespace Nexuscomm.API.Exceptions
{
    // Base exception with an HTTP status code attached, so middleware knows what to return
    public abstract class AppException : Exception
    {
        public int StatusCode { get; }

        protected AppException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }

    public class ValidationException : AppException
    {
        public List<string> Errors { get; }

        public ValidationException(string message) : base(message, 400)
        {
            Errors = new List<string> { message };
        }

        public ValidationException(List<string> errors) : base("Validation failed", 400)
        {
            Errors = errors;
        }
    }

    public class NotFoundException : AppException
    {
        public NotFoundException(string message) : base(message, 404) { }
    }

    public class ForbiddenException : AppException
    {
        public ForbiddenException(string message = "You do not have access to this resource")
            : base(message, 403) { }
    }
}