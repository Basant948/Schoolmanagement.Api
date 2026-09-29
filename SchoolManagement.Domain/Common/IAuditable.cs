namespace SchoolManagement.Domain.Common;

/// <summary>
/// Entities implementing this get Created/Updated stamps filled automatically
/// by <c>AuditableEntityInterceptor</c> and are tracked in the AuditLogs table.
/// </summary>
public interface IAuditable
{
    DateTime CreatedAtUtc { get; set; }
    string? CreatedBy { get; set; }
    DateTime? UpdatedAtUtc { get; set; }
    string? UpdatedBy { get; set; }
}
