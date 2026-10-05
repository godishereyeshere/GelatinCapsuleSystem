using GelatinCapsule.Domain.Entities.Melting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GelatinCapsule.Infrastructure.Persistence.Configurations;

public class ProductFormulaConfiguration : IEntityTypeConfiguration<ProductFormula>
{
    public void Configure(EntityTypeBuilder<ProductFormula> builder)
    {
        builder.ToTable("ProductFormulas");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Percentage).HasPrecision(18, 8);

        // ارتباط با ProductionType
        builder.HasOne(x => x.ProductionType)
               .WithMany(p => p.Formulas)
               .HasForeignKey(x => x.ProductionTypeId)
               .OnDelete(DeleteBehavior.Cascade);

        // ارتباط با Material
        builder.HasOne(x => x.Material)
               .WithMany()
               .HasForeignKey(x => x.MaterialId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.ProductionTypeId, x.MaterialId }).IsUnique();
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}