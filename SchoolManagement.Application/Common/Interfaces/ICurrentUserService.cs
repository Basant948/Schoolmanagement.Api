namespace SchoolManagement.Application.Common.Interfaces;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? UserName { get; }
    string? IpAddress { get; }
    bool IsAuthenticated { get; }
}
