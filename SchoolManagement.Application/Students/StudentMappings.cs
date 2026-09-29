using SchoolManagement.Application.Students.Dtos;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Students;

public static class StudentMappings
{
    public static StudentResponse ToResponse(this Student s) => new()
    {
        Id = s.Id,
        AdmissionNumber = s.AdmissionNumber,
        FirstName = s.FirstName,
        LastName = s.LastName,
        FullName = s.FullName,
        DateOfBirth = s.DateOfBirth,
        Gender = s.Gender,
        Email = s.Email,
        PhoneNumber = s.PhoneNumber,
        Address = s.Address,
        Grade = s.Grade,
        Section = s.Section,
        EnrollmentDate = s.EnrollmentDate,
        Status = s.Status,
        GuardianName = s.GuardianName,
        GuardianPhone = s.GuardianPhone,
        CreatedAtUtc = s.CreatedAtUtc,
        UpdatedAtUtc = s.UpdatedAtUtc
    };
}
