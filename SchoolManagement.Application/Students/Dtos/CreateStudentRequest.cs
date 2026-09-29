using System.ComponentModel.DataAnnotations;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Application.Students.Dtos;

public class CreateStudentRequest
{
    [Required, MaxLength(50)]
    public string AdmissionNumber { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    public required DateOnly DateOfBirth { get; set; }
    public required Gender Gender { get; set; }

    [EmailAddress, MaxLength(256)]
    public string? Email { get; set; }

    [Phone, MaxLength(30)]
    public string? PhoneNumber { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    [Required, MaxLength(20)]
    public string Grade { get; set; } = string.Empty;

    [MaxLength(10)]
    public string? Section { get; set; }

    /// <summary>Defaults to today when omitted.</summary>
    public DateOnly? EnrollmentDate { get; set; }

    [Required, MaxLength(200)]
    public string GuardianName { get; set; } = string.Empty;

    [Required, Phone, MaxLength(30)]
    public string GuardianPhone { get; set; } = string.Empty;
}
