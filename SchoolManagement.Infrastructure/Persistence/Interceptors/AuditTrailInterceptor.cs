using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Writes a row to AuditLogs (who / what / when / old + new values) for every change to an
/// <see cref="IAuditable"/> entity. Must be registered AFTER the soft-delete and auditable interceptors.
/// </summary>
public sealed class AuditTrailInterceptor(ICurrentUserService currentUser, TimeProvider timeProvider)
    : SaveChangesInterceptor
{
    // Never written to the audit log (secrets + noise already covered by the audit columns).
    private static readonly HashSet<string> IgnoredProperties =
    [
        "PasswordHash", "SecurityStamp", "ConcurrencyStamp",
        nameof(IAuditable.CreatedAtUtc), nameof(IAuditable.CreatedBy),
        nameof(IAuditable.UpdatedAtUtc), nameof(IAuditable.UpdatedBy),
        nameof(ISoftDeletable.DeletedAtUtc), nameof(ISoftDeletable.DeletedBy)
    ];

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

        var entries = context.ChangeTracker.Entries<IAuditable>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToList();

        foreach (var entry in entries)
        {
            var log = BuildLog(entry, now);
            if (log is not null)
                context.Set<AuditLog>().Add(log);
        }
    }

    private AuditLog? BuildLog(EntityEntry entry, DateTime now)
    {
        var oldValues = new Dictionary<string, object?>();
        var newValues = new Dictionary<string, object?>();

        foreach (var property in entry.Properties)
        {
            var name = property.Metadata.Name;
            if (property.Metadata.IsPrimaryKey() || IgnoredProperties.Contains(name)) continue;

            switch (entry.State)
            {
                case EntityState.Added:
                    newValues[name] = property.CurrentValue;
                    break;

                case EntityState.Deleted:
                    oldValues[name] = property.OriginalValue;
                    break;

                case EntityState.Modified when property.IsModified &&
                                               !Equals(property.OriginalValue, property.CurrentValue):
                    oldValues[name] = property.OriginalValue;
                    newValues[name] = property.CurrentValue;
                    break;
            }
        }

        // Modified, but only audit columns changed -> nothing worth logging.
        if (entry.State == EntityState.Modified && newValues.Count == 0) return null;

        return new AuditLog
        {
            EntityName = entry.Metadata.ClrType.Name,
            EntityId = string.Join(",", entry.Metadata.FindPrimaryKey()!.Properties
                .Select(p => entry.Property(p.Name).CurrentValue?.ToString())),
            Action = ResolveAction(entry),
            OldValues = oldValues.Count == 0 ? null : JsonSerializer.Serialize(oldValues),
            NewValues = newValues.Count == 0 ? null : JsonSerializer.Serialize(newValues),
            UserId = currentUser.UserId ?? "system",
            IpAddress = currentUser.IpAddress,
            TimestampUtc = now
        };
    }

    private static AuditAction ResolveAction(EntityEntry entry)
    {
        switch (entry.State)
        {
            case EntityState.Added:
                return AuditAction.Create;
            case EntityState.Deleted:
                return AuditAction.Delete;
        }

        if (entry.Entity is ISoftDeletable)
        {
            var isDeleted = entry.Property(nameof(ISoftDeletable.IsDeleted));
            if (isDeleted.IsModified)
            {
                return (bool)isDeleted.CurrentValue! ? AuditAction.SoftDelete : AuditAction.Restore;
            }
        }

        return AuditAction.Update;
    }
}
