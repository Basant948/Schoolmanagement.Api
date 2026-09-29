namespace SchoolManagement.Application.Common.Exceptions;

public class ForbiddenException(string message = "You do not have permission to perform this action.")
    : AppException(message);
