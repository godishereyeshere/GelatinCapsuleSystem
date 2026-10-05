using GelatinCapsule.Domain.Entities.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GelatinCapsule.Infrastructure.Persistence.Configurations;

public class RoleFormPermissionConfiguration : IEntityTypeConfiguration<RoleFormPermission>
{
    public void Configure(EntityTypeBuilder<RoleFormPermission> builder)
    {
        builder.ToTable("RoleFormPermissions");
        builder.HasKey(x => new { x.RoleId, x.FormId, x.PermissionId });

        builder.HasOne(x => x.Role)
               .WithMany(r => r.FormPermissions)
               .HasForeignKey(x => x.RoleId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Form)
               .WithMany(f => f.RolePermissions)
               .HasForeignKey(x => x.FormId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Permission)
               .WithMany()
               .HasForeignKey(x => x.PermissionId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}