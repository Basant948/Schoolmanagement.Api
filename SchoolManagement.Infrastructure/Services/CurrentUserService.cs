using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SchoolManagement.Application.Common.Interfaces;

namespace SchoolManagement.Infrastructure.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private ClaimsPrincipal? Principal => httpContextAccessor.HttpContext?.User;

    public string? UserId =>
        Principal?.FindFirstValue(ClaimTypes.NameIdentifier) ?? Principal?.FindFirstValue("sub");

    public string? UserName => Principal?.Identity?.Name;

    public string? IpAddress => httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated ?? false;
}
