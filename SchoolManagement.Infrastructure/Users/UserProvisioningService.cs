using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SchoolManagement.Application.Common.Exceptions;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Users;
using SchoolManagement.Application.Users.Dtos;
using SchoolManagement.Domain.Constants;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Identity;

namespace SchoolManagement.Infrastructure.Users;

public sealed class UserProvisioningService(
    UserManager<ApplicationUser> userManager,
    IUnitOfWork unitOfWork,
    IEmailService emailService,
    IPasswordGenerator passwordGenerator,
    IConfiguration configuration,
    ILogger<UserProvisioningService> logger) : IUserProvisioningService
{
    public async Task<CreateUserResponse> CreateUserAsync(
        CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        ValidateRole(request.TargetRole);

        var email = request.Email.Trim();
        if (await userManager.FindByEmailAsync(email) is not null)
            throw new ConflictException($"A user with email '{email}' already exists.");

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = request.FirstName?.Trim() ?? string.Empty,
            LastName = request.LastName?.Trim() ?? string.Empty,
            PhoneNumber = NullIfEmpty(request.PhoneNumber),
            JobDesignation = NullIfEmpty(request.JobDesignation),
            Qualification = NullIfEmpty(request.Qualification),
            Department = NullIfEmpty(request.Department),

            // Manual accounts skip email verification entirely; invited ones confirm via the link.
            EmailConfirmed = request.ProvisioningMethod == ProvisioningMethod.Manual
        };

        string? generatedPassword = null;
        string? invitationLink = null;

        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            IdentityResult createResult;

            if (request.ProvisioningMethod == ProvisioningMethod.Invitation)
            {
                // No password yet - the user sets one themselves via the emailed link.
                createResult = await userManager.CreateAsync(user);
            }
            else
            {
                generatedPassword = passwordGenerator.Generate();
                createResult = await userManager.CreateAsync(user, generatedPassword);
            }

            ThrowIfFailed(createResult, "create the user");
            ThrowIfFailed(await userManager.AddToRoleAsync(user, request.TargetRole), "assign the role");

            if (request.TargetRole == Roles.Parent && request.StudentAdmissionNumbers is { Count: > 0 })
                await LinkParentToStudentsAsync(user.Id, request.StudentAdmissionNumbers, cancellationToken);

            if (request.ProvisioningMethod == ProvisioningMethod.Invitation)
            {
                var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                invitationLink = BuildInvitationLink(user.Id, token);
            }

            await unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }

        var message = "User account created successfully.";

        if (invitationLink is not null)
        {
            try
            {
                await emailService.SendInvitationEmailAsync(
                    user.Email!, user.FirstName, request.TargetRole, invitationLink, cancellationToken);
                message = "Invitation email sent successfully.";
            }
            catch (EmailDeliveryException ex)
            {
                // The account already exists at this point - only the email failed. Don't roll that
                // back; tell the admin so they can resend or fall back to manual setup.
                logger.LogWarning(ex, "Invitation email failed for {Email}", user.Email);
                message = "User account created, but the invitation email could not be sent. " +
                          "Please retry, or use Manual Account Setup instead.";
            }
        }

        return new CreateUserResponse
        {
            UserId = user.Id,
            Email = user.Email!,
            Role = request.TargetRole,
            Message = message,
            GeneratedPassword = generatedPassword
        };
    }

    private async Task LinkParentToStudentsAsync(
        Guid parentUserId, List<string> identifiers, CancellationToken cancellationToken)
    {
        var admissionNumbers = identifiers
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (admissionNumbers.Count == 0) return;

        var students = await unitOfWork.Students.GetByAdmissionNumbersAsync(admissionNumbers, cancellationToken);

        var missing = admissionNumbers
            .Except(students.Select(s => s.AdmissionNumber), StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (missing.Count > 0)
        {
            throw new ValidationException(
                "studentAdmissionNumbers",
                $"No student found for admission number(s): {string.Join(", ", missing)}.");
        }

        foreach (var student in students)
        {
            await unitOfWork.ParentStudentLinks.AddAsync(
                new ParentStudentLink { ParentUserId = parentUserId, StudentId = student.Id },
                cancellationToken);
        }
    }

    private string BuildInvitationLink(Guid userId, string token)
    {
        var baseUrl = configuration["App:InvitationBaseUrl"]?.TrimEnd('/');
        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new InvalidOperationException("App:InvitationBaseUrl is not configured.");

        // Identity tokens can contain characters that aren't URL-safe as-is; Base64Url-encode them.
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        return $"{baseUrl}?userId={userId}&token={Uri.EscapeDataString(encodedToken)}";
    }

    private static void ValidateRole(string role)
    {
        if (!Roles.All.Contains(role))
        {
            throw new ValidationException(
                nameof(CreateUserRequest.TargetRole),
                $"'{role}' is not a valid role. Allowed roles: {string.Join(", ", Roles.All)}.");
        }
    }

    private static void ThrowIfFailed(IdentityResult result, string action)
    {
        if (result.Succeeded) return;
        var errors = string.Join("; ", result.Errors.Select(e => e.Description));
        throw new ValidationException("identity", $"Failed to {action}: {errors}");
    }

    private static string? NullIfEmpty(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
