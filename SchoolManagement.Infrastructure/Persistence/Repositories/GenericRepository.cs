using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Models;
using SchoolManagement.Domain.Common;

namespace SchoolManagement.Infrastructure.Persistence.Repositories;

public class GenericRepository<T>(ApplicationDbContext context) : IGenericRepository<T> where T : BaseEntity
{
    protected readonly ApplicationDbContext Context = context;
    protected readonly DbSet<T> DbSet = context.Set<T>();

    // FirstOrDefault (not Find) so the soft-delete query filter is always applied.
    public virtual Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        DbSet.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await DbSet.AsNoTracking().ToListAsync(cancellationToken);

    public virtual async Task<IReadOnlyList<T>> FindAsync(
        Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) =>
        await DbSet.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);

    public virtual Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) =>
        DbSet.FirstOrDefaultAsync(predicate, cancellationToken);

    public virtual Task<bool> AnyAsync(
        Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) =>
        DbSet.AnyAsync(predicate, cancellationToken);

    public virtual Task<int> CountAsync(
        Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default) =>
        predicate is null ? DbSet.CountAsync(cancellationToken) : DbSet.CountAsync(predicate, cancellationToken);

    public virtual async Task<PagedResult<T>> GetPagedAsync(
        int page,
        int pageSize,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = DbSet.AsNoTracking();

        if (predicate is not null)
            query = query.Where(predicate);

        var total = await query.CountAsync(cancellationToken);

        // Stable ordering is required for consistent paging.
        query = orderBy is not null ? orderBy(query) : query.OrderBy(e => e.CreatedAtUtc).ThenBy(e => e.Id);

        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return new PagedResult<T>(items, page, pageSize, total);
    }

    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default) =>
        await DbSet.AddAsync(entity, cancellationToken);

    public virtual Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default) =>
        DbSet.AddRangeAsync(entities, cancellationToken);

    public virtual void Update(T entity)
    {
        // Tracked entities are already change-tracked; only attach detached ones.
        if (Context.Entry(entity).State == EntityState.Detached)
            DbSet.Update(entity);
    }

    public virtual void Remove(T entity) => DbSet.Remove(entity);

    public virtual void RemoveRange(IEnumerable<T> entities) => DbSet.RemoveRange(entities);
}
