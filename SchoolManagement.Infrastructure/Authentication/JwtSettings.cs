using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Infrastructure.Authentication;

public class JwtSettings
{
    public const string SectionName = "Jwt";

    /// <summary>HMAC-SHA256 signing key. At least 32 characters. Keep it in user-secrets / env vars.</summary>
    [Required, MinLength(32)]
    public string Key { get; set; } = string.Empty;

    [Required]
    public string Issuer { get; set; } = string.Empty;

    [Required]
    public string Audience { get; set; } = string.Empty;

    [Range(1, 1440)]
    public int ExpiryMinutes { get; set; } = 60;
}
