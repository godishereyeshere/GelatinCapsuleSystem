using GelatinCapsule.Domain.Common;

namespace GelatinCapsule.Domain.Entities.Melting;

public class ShiftInfo : BaseEntity
{
    // ============ اطلاعات پایه ============
    public string Farsidate { get; set; } = null!;   // 14050708
    public string Shift { get; set; } = null!;       // صبح / شب
    public string? Group { get; set; }               // A / B / C / D

    // ============ پرسنل اصلی ============
    public string? Supervisore { get; set; }         // سوپروایزر
    public string? SheftHeader { get; set; }         // سرشیفت
    public string? GelMaker1 { get; set; }           // ملتر ۱
    public string? GelMaker2 { get; set; }           // ملتر ۲
    public string? IpQc { get; set; }                // ناظر کنترل کیفی
    public string? TankWasher { get; set; }          // شوینده تانک ۱
    public string? TankWasher2 { get; set; }         // شوینده تانک ۲

    // ============ پرسنل جانشین ============
    public string? SheftHeaderRep { get; set; }
    public string? SheftHeaderRepFrom { get; set; }   // از ساعت (مثلاً "20:00")
    public string? GelMaker1Rep { get; set; }
    public string? GelMaker1RepFrom { get; set; }
    public string? GelMaker2Rep { get; set; }
    public string? GelMaker2RepFrom { get; set; }
    public string? TankWasherRep { get; set; }
    public string? TankWasherRepFrom { get; set; }

    // ============ فیلدهای سیستمی ============
    public DateTime? Mildate { get; set; }            // تاریخ میلادی
    public string? Momtime { get; set; }              // ساعت ثبت
    public string? CurrentDay { get; set; }           // روز هفته
    public string? HelpMelter { get; set; }           // ملتر کمکی
    public string? YearF { get; set; }                // سال شمسی
}