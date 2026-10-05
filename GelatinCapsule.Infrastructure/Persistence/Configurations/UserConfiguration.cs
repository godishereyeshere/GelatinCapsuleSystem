using GelatinCapsule.Domain.Entities.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GelatinCapsule.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(x => x.Id);

        // اندازه‌ی ستون‌ها
        builder.Property(x => x.Username).HasMaxLength(100).IsRequired();
        builder.Property(x => x.PasswordHash).HasMaxLength(500).IsRequired();
        builder.Property(x => x.SecurityStamp).HasMaxLength(100);
        builder.Property(x => x.FullName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.PersonnelCode).HasMaxLength(50);
        builder.Property(x => x.NationalCode).HasMaxLength(10);
        builder.Property(x => x.Email).HasMaxLength(200);
        builder.Property(x => x.Mobile).HasMaxLength(20);
        builder.Property(x => x.LastLoginIp).HasMaxLength(45);

        // Index ها
        builder.HasIndex(x => x.Username).IsUnique().HasFilter("[IsDeleted] = 0");
        builder.HasIndex(x => x.PersonnelCode);
        builder.HasIndex(x => x.DepartmentId);

        // روابط
        builder.HasOne(x => x.Department)
               .WithMany(d => d.Users)
               .HasForeignKey(x => x.DepartmentId)
               .OnDelete(DeleteBehavior.SetNull);

        // Global Query Filter — رکوردهای حذف‌شده خودکار مخفی میشن
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}