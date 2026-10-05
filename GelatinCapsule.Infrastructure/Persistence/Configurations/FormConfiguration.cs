using GelatinCapsule.Domain.Entities.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GelatinCapsule.Infrastructure.Persistence.Configurations;

public class FormConfiguration : IEntityTypeConfiguration<Form>
{
    public void Configure(EntityTypeBuilder<Form> builder)
    {
        builder.ToTable("Forms");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code).HasMaxLength(100).IsRequired();
        builder.Property(x => x.DisplayName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.ControllerName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.ActionName).HasMaxLength(100);
        builder.Property(x => x.MenuPath).HasMaxLength(300);

        builder.HasIndex(x => x.Code).IsUnique();
        builder.HasIndex(x => x.ModuleId);

        builder.HasOne(x => x.Module)
               .WithMany(m => m.Forms)
               .HasForeignKey(x => x.ModuleId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}