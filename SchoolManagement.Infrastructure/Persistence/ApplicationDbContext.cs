using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Identity;
using SchoolManagement.Infrastructure.Persistence.Extensions;

namespace SchoolManagement.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options)
{
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<ParentStudentLink> ParentStudentLinks => Set<ParentStudentLink>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Must run last so it sees every entity type (including Identity's).
        builder.ApplySoftDeleteQueryFilters();
    }
}
