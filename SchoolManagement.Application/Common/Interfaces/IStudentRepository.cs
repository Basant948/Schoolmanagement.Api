using SchoolManagement.Application.Common.Models;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Application.Common.Interfaces;

public interface IStudentRepository : IGenericRepository<Student>
{
    Task<Student?> GetByAdmissionNumberAsync(string admissionNumber, CancellationToken cancellationToken = default);

    Task<bool> AdmissionNumberExistsAsync(
        string admissionNumber, Guid? excludeStudentId = null, CancellationToken cancellationToken = default);

    Task<PagedResult<Student>> SearchAsync(
        string? search, StudentStatus? status, int page, int pageSize, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Student>> GetByAdmissionNumbersAsync(
        IEnumerable<string> admissionNumbers, CancellationToken cancellationToken = default);
}
