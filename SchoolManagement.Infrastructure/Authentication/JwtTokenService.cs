using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SchoolManagement.Application.Auth;

namespace SchoolManagement.Infrastructure.Authentication;

public sealed class JwtTokenService(IOptions<JwtSettings> options, TimeProvider timeProvider) : IJwtTokenService
{
    public const string NameClaim = "name";
    public const string RoleClaim = "role";

    private readonly JwtSettings _settings = options.Value;

    public JwtTokenResult Generate(
        Guid userId, string email, string userName, string fullName, IEnumerable<string> roles)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var expires = now.AddMinutes(_settings.ExpiryMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("preferred_username", userName),
            new(NameClaim, fullName)
        };
        claims.AddRange(roles.Select(role => new Claim(RoleClaim, role)));

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = _settings.Issuer,
            Audience = _settings.Audience,
            IssuedAt = now,
            NotBefore = now,
            Expires = expires,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key)),
                SecurityAlgorithms.HmacSha256)
        };

        var token = new JsonWebTokenHandler().CreateToken(descriptor);
        return new JwtTokenResult(token, expires);
    }
}
