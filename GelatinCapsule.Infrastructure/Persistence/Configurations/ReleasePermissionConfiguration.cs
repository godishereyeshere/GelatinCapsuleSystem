using GelatinCapsule.Domain.Entities.Melting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GelatinCapsule.Infrastructure.Persistence.Configurations;

public class ReleasePermissionConfiguration : IEntityTypeConfiguration<ReleasePermission>
{
    public void Configure(EntityTypeBuilder<ReleasePermission> builder)
    {
        builder.ToTable("ReleasePermissions");
        builder.HasKey(x => x.Id);

        // شناسه‌ها
        builder.Property(x => x.FeedRecNo).HasMaxLength(20).IsRequired();
        builder.Property(x => x.BatchNo).HasMaxLength(30).IsRequired();
        builder.Property(x => x.Farsidate).HasMaxLength(10).IsRequired();
        builder.Property(x => x.Shift).HasMaxLength(10).IsRequired();
        builder.Property(x => x.FarsiYear).HasMaxLength(10);
        builder.Property(x => x.Part).HasMaxLength(20).IsRequired();
        builder.Property(x => x.PType).HasMaxLength(30).IsRequired();
        builder.Property(x => x.CCode).HasMaxLength(20).IsRequired();
        builder.HasIndex(x => x.LicenseSerial);
        builder.HasIndex(x => x.RelatedMeltId);
        builder.Property(x => x.TankNo).HasMaxLength(50);

        // اطلاعات محصول
        builder.Property(x => x.Company).HasMaxLength(100);
        builder.Property(x => x.ProductName).HasMaxLength(200);
        builder.Property(x => x.ColorCode).HasMaxLength(20);
        builder.Property(x => x.Notes).HasMaxLength(2000);

        // Decimal precision
        builder.Property(x => x.Vol0).HasPrecision(18, 4);
        builder.Property(x => x.Vis0).HasPrecision(18, 4);
        builder.Property(x => x.Vis1).HasPrecision(18, 4);
        builder.Property(x => x.C0).HasPrecision(18, 4);
        builder.Property(x => x.C1).HasPrecision(18, 4);
        builder.Property(x => x.Vol1).HasPrecision(18, 4);
        builder.Property(x => x.Wei0).HasPrecision(18, 4);
        builder.Property(x => x.EqcCapsule).HasPrecision(18, 4);
        builder.Property(x => x.WtrNeed).HasPrecision(18, 4);

        // Index ها
        builder.HasIndex(x => x.FeedRecNo).IsUnique().HasFilter("[IsDeleted] = 0");
        builder.HasIndex(x => new { x.FarsiYear, x.Serial });
        builder.HasIndex(x => x.Farsidate);
        builder.HasIndex(x => x.CCode);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}