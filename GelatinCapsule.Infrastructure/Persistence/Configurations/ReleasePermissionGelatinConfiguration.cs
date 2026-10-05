using GelatinCapsule.Domain.Entities.Melting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GelatinCapsule.Infrastructure.Persistence.Configurations;

public class ReleasePermissionGelatinConfiguration : IEntityTypeConfiguration<ReleasePermissionGelatin>
{
    public void Configure(EntityTypeBuilder<ReleasePermissionGelatin> builder)
    {
        builder.ToTable("ReleasePermissionGelatins");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.GelatinName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Amount).HasPrecision(18, 4);
        builder.Property(x => x.BatchNo).HasMaxLength(50);

        builder.HasOne(x => x.ReleasePermission)
               .WithMany(x => x.Gelatins)
               .HasForeignKey(x => x.ReleasePermissionId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.ReleasePermissionId);
    }
}