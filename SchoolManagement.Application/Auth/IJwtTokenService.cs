namespace SchoolManagement.Application.Auth;

public sealed record JwtTokenResult(string Token, DateTime ExpiresAtUtc);

public interface IJwtTokenService
{
    JwtTokenResult Generate(Guid userId, string email, string userName, string fullName, IEnumerable<string> roles);
}
