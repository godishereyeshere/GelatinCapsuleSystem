using GelatinCapsule.Domain.Entities.Melting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GelatinCapsule.Infrastructure.Persistence.Configurations;

public class ProductionTypeConfiguration : IEntityTypeConfiguration<ProductionType>
{
    public void Configure(EntityTypeBuilder<ProductionType> builder)
    {
        builder.ToTable("ProductionTypes");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code).HasMaxLength(50).IsRequired();
        builder.Property(x => x.DisplayName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.PType).HasMaxLength(30).IsRequired();
        builder.Property(x => x.CCode).HasMaxLength(20).IsRequired();
        builder.Property(x => x.DefaultVol0).HasPrecision(18, 4);
        builder.Property(x => x.Notes).HasMaxLength(500);

        builder.HasIndex(x => x.Code).IsUnique().HasFilter("[IsDeleted] = 0");
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}