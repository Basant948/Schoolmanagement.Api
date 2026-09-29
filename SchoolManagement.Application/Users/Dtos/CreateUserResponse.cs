namespace SchoolManagement.Application.Users.Dtos;

public class CreateUserResponse
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    /// <summary>Only populated for Manual provisioning. Show it to the admin once - it is never stored.</summary>
    public string? GeneratedPassword { get; set; }
}
