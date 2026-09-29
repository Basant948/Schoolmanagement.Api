using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Users;
using SchoolManagement.Application.Users.Dtos;
using SchoolManagement.Domain.Constants;

namespace SchoolManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Admin)]
public class UsersController(IUserProvisioningService userProvisioningService) : ControllerBase
{
    /// <summary>
    /// Creates a Teacher, Parent, Accountant, Librarian or Admin account. Set "provisioningMethod"
    /// to "Invitation" (emails the user a set-password link) or "Manual" (returns a generated
    /// password immediately, no email needed). For Parent, pass the students' admission numbers
    /// in "studentAdmissionNumbers" to link them.
    /// </summary>
    [HttpPost]
    [ProducesResponseType<CreateUserResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateUserResponse>> Create(
        CreateUserRequest request, CancellationToken cancellationToken)
    {
        var result = await userProvisioningService.CreateUserAsync(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, result);
    }
}
