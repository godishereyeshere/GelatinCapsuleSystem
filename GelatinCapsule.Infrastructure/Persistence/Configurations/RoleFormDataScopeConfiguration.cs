using GelatinCapsule.Domain.Entities.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GelatinCapsule.Infrastructure.Persistence.Configurations;

public class RoleFormDataScopeConfiguration : IEntityTypeConfiguration<RoleFormDataScope>
{
    public void Configure(EntityTypeBuilder<RoleFormDataScope> builder)
    {
        builder.ToTable("RoleFormDataScopes");
        builder.HasKey(x => new { x.RoleId, x.FormId });

        builder.HasOne(x => x.Role)
               .WithMany(r => r.FormDataScopes)
               .HasForeignKey(x => x.RoleId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Form)
               .WithMany(f => f.RoleDataScopes)
               .HasForeignKey(x => x.FormId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}