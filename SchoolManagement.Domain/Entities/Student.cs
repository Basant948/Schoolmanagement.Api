using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Domain.Entities;

public class Student : BaseEntity
{
    public string AdmissionNumber { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public DateOnly DateOfBirth { get; set; }
    public Gender Gender { get; set; }

    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }

    public string Grade { get; set; } = default!;
    public string? Section { get; set; }
    public DateOnly EnrollmentDate { get; set; }
    public StudentStatus Status { get; set; } = StudentStatus.Active;

    public string GuardianName { get; set; } = default!;
    public string GuardianPhone { get; set; } = default!;

    public string FullName => $"{FirstName} {LastName}";
}
