using SchoolManagement.Application.Auth.Dtos;

namespace SchoolManagement.Application.Auth;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

    /// <summary>Confirms the invited user's email and sets their first password.</summary>
    Task AcceptInvitationAsync(AcceptInvitationRequest request, CancellationToken cancellationToken = default);
}
