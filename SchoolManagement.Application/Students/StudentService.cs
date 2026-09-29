using SchoolManagement.Application.Common.Exceptions;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Models;
using SchoolManagement.Application.Students.Dtos;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Students;

public class StudentService(IUnitOfWork unitOfWork, TimeProvider timeProvider) : IStudentService
{
    private const int MaxPageSize = 100;

    public async Task<PagedResult<StudentResponse>> GetPagedAsync(
        StudentQuery query, CancellationToken cancellationToken = default)
    {
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, MaxPageSize);

        var result = await unitOfWork.Students.SearchAsync(query.Search, query.Status, page, pageSize, cancellationToken);
        return result.Map(s => s.ToResponse());
    }

    public async Task<StudentResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var student = await GetOrThrowAsync(id, cancellationToken);
        return student.ToResponse();
    }

    public async Task<StudentResponse> CreateAsync(
        CreateStudentRequest request, CancellationToken cancellationToken = default)
    {
        ValidateDateOfBirth(request.DateOfBirth);

        var admissionNumber = request.AdmissionNumber.Trim();
        await EnsureAdmissionNumberIsUniqueAsync(admissionNumber, null, cancellationToken);

        var student = new Student
        {
            AdmissionNumber = admissionNumber,
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            Email = NullIfEmpty(request.Email),
            PhoneNumber = NullIfEmpty(request.PhoneNumber),
            Address = NullIfEmpty(request.Address),
            Grade = request.Grade.Trim(),
            Section = NullIfEmpty(request.Section),
            EnrollmentDate = request.EnrollmentDate ?? Today(),
            GuardianName = request.GuardianName.Trim(),
            GuardianPhone = request.GuardianPhone.Trim()
        };

        await unitOfWork.Students.AddAsync(student, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return student.ToResponse();
    }

    public async Task<StudentResponse> UpdateAsync(
        Guid id, UpdateStudentRequest request, CancellationToken cancellationToken = default)
    {
        ValidateDateOfBirth(request.DateOfBirth);

        var student = await GetOrThrowAsync(id, cancellationToken);

        var admissionNumber = request.AdmissionNumber.Trim();
        await EnsureAdmissionNumberIsUniqueAsync(admissionNumber, id, cancellationToken);

        student.AdmissionNumber = admissionNumber;
        student.FirstName = request.FirstName.Trim();
        student.LastName = request.LastName.Trim();
        student.DateOfBirth = request.DateOfBirth;
        student.Gender = request.Gender;
        student.Email = NullIfEmpty(request.Email);
        student.PhoneNumber = NullIfEmpty(request.PhoneNumber);
        student.Address = NullIfEmpty(request.Address);
        student.Grade = request.Grade.Trim();
        student.Section = NullIfEmpty(request.Section);
        student.EnrollmentDate = request.EnrollmentDate;
        student.Status = request.Status;
        student.GuardianName = request.GuardianName.Trim();
        student.GuardianPhone = request.GuardianPhone.Trim();

        unitOfWork.Students.Update(student);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return student.ToResponse();
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var student = await GetOrThrowAsync(id, cancellationToken);

        unitOfWork.Students.Remove(student); // soft delete
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    // ---- helpers ----

    private async Task<Student> GetOrThrowAsync(Guid id, CancellationToken cancellationToken) =>
        await unitOfWork.Students.GetByIdAsync(id, cancellationToken)
        ?? throw new NotFoundException(nameof(Student), id);

    private async Task EnsureAdmissionNumberIsUniqueAsync(
        string admissionNumber, Guid? excludeId, CancellationToken cancellationToken)
    {
        if (await unitOfWork.Students.AdmissionNumberExistsAsync(admissionNumber, excludeId, cancellationToken))
            throw new ConflictException($"A student with admission number '{admissionNumber}' already exists.");
    }

    private void ValidateDateOfBirth(DateOnly dateOfBirth)
    {
        if (dateOfBirth > Today())
            throw new ValidationException(nameof(dateOfBirth), "Date of birth cannot be in the future.");
    }

    private DateOnly Today() => DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);

    private static string? NullIfEmpty(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
