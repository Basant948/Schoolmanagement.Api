using SchoolManagement.Domain.Common;

namespace SchoolManagement.Application.Common.Interfaces;

public interface IUnitOfWork
{
    IStudentRepository Students { get; }
    IParentStudentLinkRepository ParentStudentLinks { get; }

    /// <summary>Generic repository for any entity that doesn't need a specialised repository yet.</summary>
    IGenericRepository<T> Repository<T>() where T : BaseEntity;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
