namespace SchoolManagement.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendInvitationEmailAsync(
        string toEmail,
        string firstName,
        string role,
        string acceptInvitationUrl,
        CancellationToken cancellationToken = default);
}
