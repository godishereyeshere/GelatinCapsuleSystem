using GelatinCapsule.Domain.Entities.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GelatinCapsule.Infrastructure.Persistence.Configurations;

public class RoleModuleAccessConfiguration : IEntityTypeConfiguration<RoleModuleAccess>
{
    public void Configure(EntityTypeBuilder<RoleModuleAccess> builder)
    {
        builder.ToTable("RoleModuleAccesses");
        builder.HasKey(x => new { x.RoleId, x.ModuleId });

        builder.HasOne(x => x.Role)
               .WithMany(r => r.ModuleAccesses)
               .HasForeignKey(x => x.RoleId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Module)
               .WithMany(m => m.RoleAccesses)
               .HasForeignKey(x => x.ModuleId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}