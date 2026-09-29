namespace SchoolManagement.Application.Common.Exceptions;

/// <summary>Authentication failed (bad credentials, locked account, ...). Maps to HTTP 401.</summary>
public class UnauthorizedException(string message = "Authentication failed.") : AppException(message);
