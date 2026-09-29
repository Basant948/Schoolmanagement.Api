using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Infrastructure.Email;

public class ResendSettings
{
    public const string SectionName = "Resend";

    [Required]
    public string ApiKey { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string FromEmail { get; set; } = string.Empty;

    [Required]
    public string FromName { get; set; } = string.Empty;
}
