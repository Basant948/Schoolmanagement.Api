using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Common.Interfaces;

public interface IParentStudentLinkRepository : IGenericRepository<ParentStudentLink>
{
    Task<IReadOnlyList<Guid>> GetStudentIdsForParentAsync(
        Guid parentUserId, CancellationToken cancellationToken = default);

    Task<bool> LinkExistsAsync(
        Guid parentUserId, Guid studentId, CancellationToken cancellationToken = default);
}
