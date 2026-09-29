using Microsoft.AspNetCore.Identity;
using SchoolManagement.Domain.Common;

namespace SchoolManagement.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<Guid>, IAuditable, ISoftDeletable
{
    public ApplicationUser()
    {
        Id = Guid.CreateVersion7();
    }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    // Optional role-specific fields, filled in by user provisioning.
    public string? JobDesignation { get; set; } // Accountant / Admin / Librarian
    public string? Qualification { get; set; }  // Teacher
    public string? Department { get; set; }     // Teacher

    public DateTime CreatedAtUtc { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public string? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
    public string? DeletedBy { get; set; }
}
