using Microsoft.EntityFrameworkCore.Storage;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Domain.Common;
using SchoolManagement.Infrastructure.Persistence.Repositories;

namespace SchoolManagement.Infrastructure.Persistence;

public sealed class UnitOfWork(ApplicationDbContext context) : IUnitOfWork, IAsyncDisposable
{
    private readonly Dictionary<Type, object> _repositories = [];
    private IStudentRepository? _students;
    private IParentStudentLinkRepository? _parentStudentLinks;
    private IDbContextTransaction? _transaction;

    public IStudentRepository Students => _students ??= new StudentRepository(context);
    public IParentStudentLinkRepository ParentStudentLinks => _parentStudentLinks ??= new ParentStudentLinkRepository(context);

    public IGenericRepository<T> Repository<T>() where T : BaseEntity
    {
        if (!_repositories.TryGetValue(typeof(T), out var repository))
        {
            repository = new GenericRepository<T>(context);
            _repositories[typeof(T)] = repository;
        }

        return (IGenericRepository<T>)repository;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is not null)
            throw new InvalidOperationException("A transaction is already in progress.");

        _transaction = await context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is null)
            throw new InvalidOperationException("No transaction in progress.");

        try
        {
            await context.SaveChangesAsync(cancellationToken);
            await _transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is null) return;

        try
        {
            await _transaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    private async Task DisposeTransactionAsync()
    {
        if (_transaction is null) return;
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async ValueTask DisposeAsync() => await DisposeTransactionAsync();
}
