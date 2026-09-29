using SchoolManagement.Application.Common.Models;
using SchoolManagement.Application.Students.Dtos;

namespace SchoolManagement.Application.Students;

public interface IStudentService
{
    Task<PagedResult<StudentResponse>> GetPagedAsync(StudentQuery query, CancellationToken cancellationToken = default);
    Task<StudentResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<StudentResponse> CreateAsync(CreateStudentRequest request, CancellationToken cancellationToken = default);
    Task<StudentResponse> UpdateAsync(Guid id, UpdateStudentRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
