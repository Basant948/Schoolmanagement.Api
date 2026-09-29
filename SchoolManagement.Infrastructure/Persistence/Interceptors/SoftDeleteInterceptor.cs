using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Domain.Common;

namespace SchoolManagement.Infrastructure.Persistence.Interceptors;

/// <summary>Turns <c>Remove(entity)</c> on an <see cref="ISoftDeletable"/> into an UPDATE.</summary>
public sealed class SoftDeleteInterceptor(ICurrentUserService currentUser, TimeProvider timeProvider)
    : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData, InterceptionResult<int> result)
    {
        Apply(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        Apply(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void Apply(DbContext? context)
    {
        if (context is null) return;

        var now = timeProvider.GetUtcNow().UtcDateTime;
        var user = currentUser.UserId ?? "system";

        foreach (var entry in context.ChangeTracker.Entries<ISoftDeletable>()
                     .Where(e => e.State == EntityState.Deleted))
        {
            entry.State = EntityState.Modified;
            entry.Entity.IsDeleted = true;
            entry.Entity.DeletedAtUtc = now;
            entry.Entity.DeletedBy = user;
        }
    }
}
