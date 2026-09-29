using SchoolManagement.Application.Users.Dtos;

namespace SchoolManagement.Application.Users;

public interface IUserProvisioningService
{
    Task<CreateUserResponse> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
}
