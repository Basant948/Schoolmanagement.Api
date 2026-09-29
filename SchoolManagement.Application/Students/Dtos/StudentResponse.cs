using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Application.Students.Dtos;

public class StudentResponse
{
    public Guid Id { get; set; }
    public string AdmissionNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string Grade { get; set; } = string.Empty;
    public string? Section { get; set; }
    public DateOnly EnrollmentDate { get; set; }
    public StudentStatus Status { get; set; }
    public string GuardianName { get; set; } = string.Empty;
    public string GuardianPhone { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}
