using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Identity;

namespace SchoolManagement.Infrastructure.Persistence.Configurations;

public class ParentStudentLinkConfiguration : IEntityTypeConfiguration<ParentStudentLink>
{
    public void Configure(EntityTypeBuilder<ParentStudentLink> builder)
    {
        builder.ToTable("ParentStudentLinks");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.UpdatedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);

        // A parent is linked to the same student only once among active (non-deleted) links.
        builder.HasIndex(x => new { x.ParentUserId, x.StudentId })
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(x => x.ParentUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Student>()
            .WithMany()
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
