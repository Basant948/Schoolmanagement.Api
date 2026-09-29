using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Application.Users.Dtos;

public class CreateUserRequest
{
    [Required]
    public ProvisioningMethod ProvisioningMethod { get; set; }

    /// <summary>Teacher, Parent, Accountant, Librarian or Admin - validated against SchoolManagement.Domain.Constants.Roles.</summary>
    [Required]
    public string TargetRole { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? FirstName { get; set; }

    [MaxLength(100)]
    public string? LastName { get; set; }

    [Phone, MaxLength(30)]
    public string? PhoneNumber { get; set; }

    // Teacher-specific
    [MaxLength(200)]
    public string? Qualification { get; set; }

    [MaxLength(100)]
    public string? Department { get; set; }

    // Accountant / Admin / Librarian
    [MaxLength(100)]
    public string? JobDesignation { get; set; }

    /// <summary>Parent-only: admission numbers of the student(s) to link to this parent.</summary>
    public List<string>? StudentAdmissionNumbers { get; set; }
}
