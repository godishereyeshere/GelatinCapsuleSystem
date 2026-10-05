using GelatinCapsule.Domain.Entities.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GelatinCapsule.Infrastructure.Persistence.Configurations;

public class UserFormPermissionConfiguration : IEntityTypeConfiguration<UserFormPermission>
{
    public void Configure(EntityTypeBuilder<UserFormPermission> builder)
    {
        builder.ToTable("UserFormPermissions");
        builder.HasKey(x => new { x.UserId, x.FormId, x.PermissionId });

        builder.HasOne(x => x.User)
               .WithMany(u => u.FormPermissions)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Form)
               .WithMany(f => f.UserPermissions)
               .HasForeignKey(x => x.FormId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Permission)
               .WithMany()
               .HasForeignKey(x => x.PermissionId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}