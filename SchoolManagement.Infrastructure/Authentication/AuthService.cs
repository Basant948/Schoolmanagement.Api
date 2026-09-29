using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using SchoolManagement.Application.Auth;
using SchoolManagement.Application.Auth.Dtos;
using SchoolManagement.Application.Common.Exceptions;
using SchoolManagement.Infrastructure.Identity;

namespace SchoolManagement.Infrastructure.Authentication;

public sealed class AuthService(
    UserManager<ApplicationUser> userManager,
    IJwtTokenService jwtTokenService) : IAuthService
{
    // Same message for "no such user" and "wrong password" so accounts can't be enumerated.
    private const string InvalidCredentials = "Invalid email or password.";

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(request.Email.Trim())
                   ?? throw new UnauthorizedException(InvalidCredentials);

        if (await userManager.IsLockedOutAsync(user))
            throw new UnauthorizedException("Account is temporarily locked. Please try again later.");

        if (!await userManager.CheckPasswordAsync(user, request.Password))
        {
            await userManager.AccessFailedAsync(user); // counts toward lockout
            throw new UnauthorizedException(InvalidCredentials);
        }

        if (user.AccessFailedCount > 0)
            await userManager.ResetAccessFailedCountAsync(user);

        var roles = (await userManager.GetRolesAsync(user)).ToList();
        var fullName = $"{user.FirstName} {user.LastName}".Trim();
        if (fullName.Length == 0) fullName = user.UserName ?? user.Email!;

        var token = jwtTokenService.Generate(user.Id, user.Email!, user.UserName ?? user.Email!, fullName, roles);

        return new AuthResponse
        {
            AccessToken = token.Token,
            ExpiresAtUtc = token.ExpiresAtUtc,
            User = new UserInfo
            {
                Id = user.Id,
                Email = user.Email!,
                UserName = user.UserName ?? user.Email!,
                FullName = fullName,
                Roles = roles
            }
        };
    }

    public async Task AcceptInvitationAsync(
        AcceptInvitationRequest request, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString())
            ?? throw new UnauthorizedException("This invitation link is invalid or has expired.");

        if (await userManager.HasPasswordAsync(user))
            throw new ConflictException("This account has already been activated. Please log in instead.");

        string token;
        try
        {
            token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));
        }
        catch (FormatException)
        {
            throw new UnauthorizedException("This invitation link is invalid or has expired.");
        }

        var confirmResult = await userManager.ConfirmEmailAsync(user, token);
        if (!confirmResult.Succeeded)
            throw new UnauthorizedException("This invitation link is invalid or has expired.");

        var addPasswordResult = await userManager.AddPasswordAsync(user, request.Password);
        if (!addPasswordResult.Succeeded)
        {
            var errors = string.Join("; ", addPasswordResult.Errors.Select(e => e.Description));
            throw new ValidationException("password", errors);
        }
    }
}
