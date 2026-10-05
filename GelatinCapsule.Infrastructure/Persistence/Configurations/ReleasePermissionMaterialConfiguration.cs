using GelatinCapsule.Domain.Entities.Melting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GelatinCapsule.Infrastructure.Persistence.Configurations;

public class ReleasePermissionMaterialConfiguration : IEntityTypeConfiguration<ReleasePermissionMaterial>
{
    public void Configure(EntityTypeBuilder<ReleasePermissionMaterial> builder)
    {
        builder.ToTable("ReleasePermissionMaterials");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.MaterialCode).HasMaxLength(20).IsRequired();
        builder.Property(x => x.RequiredGr).HasPrecision(18, 4);

        builder.HasOne(x => x.ReleasePermission)
               .WithMany(x => x.Materials)
               .HasForeignKey(x => x.ReleasePermissionId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.ReleasePermissionId);
    }
}