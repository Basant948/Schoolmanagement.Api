using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Infrastructure.Persistence.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("Students");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.AdmissionNumber).HasMaxLength(50).IsRequired();
        builder.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.LastName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Email).HasMaxLength(256);
        builder.Property(x => x.PhoneNumber).HasMaxLength(30);
        builder.Property(x => x.Address).HasMaxLength(500);
        builder.Property(x => x.Grade).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Section).HasMaxLength(10);
        builder.Property(x => x.GuardianName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.GuardianPhone).HasMaxLength(30).IsRequired();

        builder.Property(x => x.Gender).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);

        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.UpdatedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);

        // Unique only among non-deleted rows, so an admission number can be reused after a soft delete.
        builder.HasIndex(x => x.AdmissionNumber)
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");

        builder.HasIndex(x => new { x.LastName, x.FirstName });
        builder.HasIndex(x => x.Status);
    }
}
