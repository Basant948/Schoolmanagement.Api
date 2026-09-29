using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Infrastructure.Persistence.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.EntityName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.EntityId).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Action).HasConversion<string>().HasMaxLength(20);
        builder.Property(x => x.OldValues).HasColumnType("jsonb");
        builder.Property(x => x.NewValues).HasColumnType("jsonb");
        builder.Property(x => x.UserId).HasMaxLength(100);
        builder.Property(x => x.IpAddress).HasMaxLength(64);

        builder.HasIndex(x => new { x.EntityName, x.EntityId });
        builder.HasIndex(x => x.TimestampUtc);
        builder.HasIndex(x => x.UserId);
    }
}
