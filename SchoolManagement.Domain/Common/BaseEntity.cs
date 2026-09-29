namespace SchoolManagement.Domain.Common;

/// <summary>
/// Base class for every domain entity: GUID v7 key + audit stamps + soft delete.
/// </summary>
public abstract class BaseEntity : IAuditable, ISoftDeletable
{
    public Guid Id { get; set; } = Guid.CreateVersion7();

    public DateTime CreatedAtUtc { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public string? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
    public string? DeletedBy { get; set; }
}
