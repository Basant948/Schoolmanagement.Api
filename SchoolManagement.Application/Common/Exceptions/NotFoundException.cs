namespace SchoolManagement.Application.Common.Exceptions;

public class NotFoundException : AppException
{
    public NotFoundException(string message) : base(message) { }

    public NotFoundException(string entityName, object key)
        : base($"{entityName} with id '{key}' was not found.") { }
}
