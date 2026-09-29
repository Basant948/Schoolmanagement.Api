using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Common;

namespace SchoolManagement.Infrastructure.Persistence.Extensions;

public static class ModelBuilderExtensions
{
    /// <summary>
    /// Adds <c>WHERE IsDeleted = false</c> to every entity implementing <see cref="ISoftDeletable"/>.
    /// Use <c>.IgnoreQueryFilters()</c> to include deleted rows.
    /// </summary>
    public static ModelBuilder ApplySoftDeleteQueryFilters(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // Derived types inherit the filter from their root type.
            if (entityType.BaseType is not null ||
                !typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                continue;
            }

            var parameter = Expression.Parameter(entityType.ClrType, "e");
            var isDeleted = Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));
            var filter = Expression.Lambda(Expression.Equal(isDeleted, Expression.Constant(false)), parameter);

            modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
        }

        return modelBuilder;
    }
}
