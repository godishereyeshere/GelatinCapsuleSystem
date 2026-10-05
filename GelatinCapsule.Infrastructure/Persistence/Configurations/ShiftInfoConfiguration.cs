using GelatinCapsule.Domain.Entities.Melting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GelatinCapsule.Infrastructure.Persistence.Configurations;

public class ShiftInfoConfiguration : IEntityTypeConfiguration<ShiftInfo>
{
    public void Configure(EntityTypeBuilder<ShiftInfo> builder)
    {
        builder.ToTable("MeltingShiftInfos");
        builder.HasKey(x => x.Id);

        // اطلاعات پایه
        builder.Property(x => x.Farsidate).HasMaxLength(10).IsRequired();
        builder.Property(x => x.Shift).HasMaxLength(10).IsRequired();
        builder.Property(x => x.Group).HasMaxLength(5);

        // پرسنل اصلی
        builder.Property(x => x.Supervisore).HasMaxLength(100);
        builder.Property(x => x.SheftHeader).HasMaxLength(100);
        builder.Property(x => x.GelMaker1).HasMaxLength(100);
        builder.Property(x => x.GelMaker2).HasMaxLength(100);
        builder.Property(x => x.IpQc).HasMaxLength(100);
        builder.Property(x => x.TankWasher).HasMaxLength(100);
        builder.Property(x => x.TankWasher2).HasMaxLength(100);

        // جانشین ها
        builder.Property(x => x.SheftHeaderRep).HasMaxLength(100);
        builder.Property(x => x.SheftHeaderRepFrom).HasMaxLength(10);
        builder.Property(x => x.GelMaker1Rep).HasMaxLength(100);
        builder.Property(x => x.GelMaker1RepFrom).HasMaxLength(10);
        builder.Property(x => x.GelMaker2Rep).HasMaxLength(100);
        builder.Property(x => x.GelMaker2RepFrom).HasMaxLength(10);
        builder.Property(x => x.TankWasherRep).HasMaxLength(100);
        builder.Property(x => x.TankWasherRepFrom).HasMaxLength(10);

        // سیستمی
        builder.Property(x => x.Momtime).HasMaxLength(10);
        builder.Property(x => x.CurrentDay).HasMaxLength(20);
        builder.Property(x => x.HelpMelter).HasMaxLength(100);
        builder.Property(x => x.YearF).HasMaxLength(10);

        // Index ها
        builder.HasIndex(x => x.Farsidate);
        builder.HasIndex(x => new { x.Farsidate, x.Shift })
               .IsUnique()
               .HasFilter("[IsDeleted] = 0")
               .HasDatabaseName("UX_MeltingShiftInfo_Date_Shift");

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}