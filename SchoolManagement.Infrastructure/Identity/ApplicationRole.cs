using Microsoft.AspNetCore.Identity;
using SchoolManagement.Domain.Common;

namespace SchoolManagement.Infrastructure.Identity;

/// <summary>Roles are audited but not soft-deleted (role names must stay unique).</summary>
public class ApplicationRole : IdentityRole<Guid>, IAuditable
{
    public ApplicationRole()
    {
        Id = Guid.CreateVersion7();
    }

    public ApplicationRole(string roleName) : this()
    {
        Name = roleName;
    }

    public DateTime CreatedAtUtc { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public string? UpdatedBy { get; set; }
}
