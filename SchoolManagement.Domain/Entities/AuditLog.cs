using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Domain.Entities;

/// <summary>Immutable record of a change made to an auditable entity.</summary>
public class AuditLog
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string EntityName { get; set; } = default!;
    public string EntityId { get; set; } = default!;
    public AuditAction Action { get; set; }

    /// <summary>JSON of the previous values (only changed columns). Null for creates.</summary>
    public string? OldValues { get; set; }

    /// <summary>JSON of the new values (only changed columns). Null for deletes.</summary>
    public string? NewValues { get; set; }

    public string? UserId { get; set; }
    public string? IpAddress { get; set; }
    public DateTime TimestampUtc { get; set; }
}
