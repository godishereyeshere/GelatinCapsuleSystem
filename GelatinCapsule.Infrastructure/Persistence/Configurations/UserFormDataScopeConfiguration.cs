using GelatinCapsule.Domain.Entities.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GelatinCapsule.Infrastructure.Persistence.Configurations;

public class UserFormDataScopeConfiguration : IEntityTypeConfiguration<UserFormDataScope>
{
    public void Configure(EntityTypeBuilder<UserFormDataScope> builder)
    {
        builder.ToTable("UserFormDataScopes");
        builder.HasKey(x => new { x.UserId, x.FormId });

        builder.HasOne(x => x.User)
               .WithMany(u => u.FormDataScopes)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Form)
               .WithMany(f => f.UserDataScopes)
               .HasForeignKey(x => x.FormId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}