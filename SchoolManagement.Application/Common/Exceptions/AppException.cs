namespace SchoolManagement.Application.Common.Exceptions;

/// <summary>Base type for all expected, business-level exceptions.</summary>
public abstract class AppException(string message, Exception? innerException = null)
    : Exception(message, innerException);
