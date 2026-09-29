namespace SchoolManagement.Domain.Common;

/// <summary>
/// Entities implementing this are never physically deleted: <c>Remove()</c> is converted to
/// an update by <c>SoftDeleteInterceptor</c>, and a global query filter hides deleted rows.
/// </summary>
public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
    DateTime? DeletedAtUtc { get; set; }
    string? DeletedBy { get; set; }
}
