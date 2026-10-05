using GelatinCapsule.Domain.Entities.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GelatinCapsule.Infrastructure.Persistence.Configurations;

public class UserModuleAccessConfiguration : IEntityTypeConfiguration<UserModuleAccess>
{
    public void Configure(EntityTypeBuilder<UserModuleAccess> builder)
    {
        builder.ToTable("UserModuleAccesses");
        builder.HasKey(x => new { x.UserId, x.ModuleId });

        builder.HasOne(x => x.User)
               .WithMany(u => u.ModuleAccesses)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Module)
               .WithMany(m => m.UserAccesses)
               .HasForeignKey(x => x.ModuleId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}