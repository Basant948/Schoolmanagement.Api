using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Application.Auth.Dtos;

public class AcceptInvitationRequest
{
    [Required]
    public Guid UserId { get; set; }

    /// <summary>The URL-safe token from the invitation link (query param "token").</summary>
    [Required]
    public string Token { get; set; } = string.Empty;

    [Required, MinLength(8)]
    public string Password { get; set; } = string.Empty;
}
