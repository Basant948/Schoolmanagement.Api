namespace SchoolManagement.Application.Common.Exceptions;

/// <summary>The email provider (Resend) failed to accept or send a message. Maps to HTTP 502.</summary>
public class EmailDeliveryException(string message) : AppException(message);
