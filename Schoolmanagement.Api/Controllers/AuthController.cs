using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Auth;
using SchoolManagement.Application.Auth.Dtos;

namespace SchoolManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>Exchange email + password for a JWT access token.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken cancellationToken) =>
        Ok(await authService.LoginAsync(request, cancellationToken));

    /// <summary>Confirms the invited user's email and sets their first password (from the invitation link).</summary>
    [HttpPost("accept-invitation")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AcceptInvitation(
        AcceptInvitationRequest request, CancellationToken cancellationToken)
    {
        await authService.AcceptInvitationAsync(request, cancellationToken);
        return NoContent();
    }

    /// <summary>Returns the identity carried by the current token (handy for testing auth).</summary>
    [HttpGet("me")]
    [Authorize]
    public IActionResult Me() => Ok(new
    {
        userId = User.FindFirst("sub")?.Value,
        email = User.FindFirst("email")?.Value,
        name = User.Identity?.Name,
        roles = User.FindAll("role").Select(c => c.Value)
    });
}
