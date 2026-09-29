namespace SchoolManagement.Application.Common.Exceptions;

public class ValidationException : AppException
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(IDictionary<string, string[]> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors;
    }

    public ValidationException(string property, string message)
        : this(new Dictionary<string, string[]> { [property] = [message] }) { }
}
