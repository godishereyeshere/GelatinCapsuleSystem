using GelatinCapsule.Domain.Common;

namespace GelatinCapsule.Domain.Entities.Melting;

/// <summary>
/// مجوز ساخت ملت / پیست
/// </summary>
public class ReleasePermission : BaseEntity
{
    // ========== شناسه‌های اصلی ==========
    public string FeedRecNo { get; set; } = null!;     // 14050031080
    public string BatchNo { get; set; } = null!;       // melt1080
    public int Serial { get; set; }                    // 1080

    // ========== تاریخ و شیفت ==========
    public string Farsidate { get; set; } = null!;     // 1405/07/08
    public string Shift { get; set; } = null!;         // صبح / شب
    public string? FarsiYear { get; set; }             // 1405

    // ========== نوع تولید ==========
    public string Part { get; set; } = null!;          // melt / pst
    public string PType { get; set; } = null!;         // Gelatin / Fish / DPI / HPMC / ...
    public string CCode { get; set; } = null!;         // Y000 / W000
    public int Machine { get; set; } = 0;              // 0, 1, 2, ...
    public int Permission { get; set; }                // 8500, 85001, 85002, ...

    // ========== اطلاعات تانک ==========
    public string? TankNo { get; set; }
    public int? TankMelt1 { get; set; }
    public int? TankMelt2 { get; set; }

    // ========== ورودی‌های کاربر ==========
    public decimal Vol0 { get; set; }                  // حجم اولیه (لیتر)
    public decimal Vis0 { get; set; }                  // ویسکوزیته اولیه
    public decimal Vis1 { get; set; }                  // ویسکوزیته نهایی
    public int LagTime { get; set; } = 0;              // مدت حباب‌گیری

    // ========== محاسبات خودکار ==========
    public decimal C0 { get; set; }                    // غلظت اولیه
    public decimal C1 { get; set; }                    // غلظت نهایی
    public decimal Vol1 { get; set; }                  // حجم نهایی
    public decimal Wei0 { get; set; }                  // وزن اولیه (گرم)
    public decimal EqcCapsule { get; set; }            // معادل خشک (گرم)
    public decimal WtrNeed { get; set; }               // آب مورد نیاز (لیتر)

    // ========== اطلاعات محصول ==========
    public string? Company { get; set; }               // IGCC
    public string? ProductName { get; set; }           // زرد کهربایی Y000
    public string? ColorCode { get; set; }             // 300 / 400
    public string? Notes { get; set; }


    public int? LicenseSerial { get; set; }  // شماره ثبت مجوز (128002)
    public int? RelatedMeltId { get; set; }  // سریال ملت مربوطه (برای پیست)
    // ========== Navigation ==========
    public ICollection<ReleasePermissionMaterial> Materials { get; set; }
        = new List<ReleasePermissionMaterial>();

    public ICollection<ReleasePermissionGelatin> Gelatins { get; set; }
    = new List<ReleasePermissionGelatin>();
}