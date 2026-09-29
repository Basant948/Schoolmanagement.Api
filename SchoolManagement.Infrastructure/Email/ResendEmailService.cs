// NOTE: this targets the "Resend" NuGet package's documented shape (IResend / ResendClient /
// ResendClientOptions.ApiToken, EmailMessage.To as a list). This sandbox could not reach
// nuget.org to compile against v0.16.0, so if the real package differs, fix the type/member
// names here and in ResendConfiguration.cs - the IEmailService contract used by the rest of
// the app will not need to change.
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Resend;
using SchoolManagement.Application.Common.Exceptions;
using SchoolManagement.Application.Common.Interfaces;

namespace SchoolManagement.Infrastructure.Email;

public sealed class ResendEmailService(
    IResend resendClient,
    IOptions<ResendSettings> options,
    ILogger<ResendEmailService> logger) : IEmailService
{
    private readonly ResendSettings _settings = options.Value;

    public async Task SendInvitationEmailAsync(
        string toEmail,
        string firstName,
        string role,
        string acceptInvitationUrl,
        CancellationToken cancellationToken = default)
    {
        var message = new EmailMessage
        {
            From = $"{_settings.FromName} <{_settings.FromEmail}>",
            Subject = $"You're invited to join {_settings.FromName} as {role}"
        };
        message.To.Add(toEmail);
        message.HtmlBody = BuildHtml(firstName, role, acceptInvitationUrl);

        try
        {
            await resendClient.EmailSendAsync(message, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send invitation email to {Email}", toEmail);
            throw new EmailDeliveryException($"Could not send the invitation email to {toEmail}.");
        }
    }

    private static string BuildHtml(string firstName, string role, string url)
    {
        var greeting = string.IsNullOrWhiteSpace(firstName) ? "Hello" : $"Hello {firstName}";

        return $"""
            <div style="font-family:Arial,sans-serif;max-width:480px;margin:auto;line-height:1.5">
              <h2>{greeting},</h2>
              <p>You've been invited to join as <strong>{role}</strong>.</p>
              <p>Click the button below to confirm your email and set your password:</p>
              <p>
                <a href="{url}"
                   style="display:inline-block;background:#14b8a6;color:#ffffff;padding:10px 20px;
                          border-radius:6px;text-decoration:none;font-weight:bold">
                  Set your password
                </a>
              </p>
              <p>If the button doesn't work, copy this link into your browser:<br>{url}</p>
              <p style="color:#6b7280;font-size:12px">This link can only be used once.</p>
            </div>
            """;
    }
}
