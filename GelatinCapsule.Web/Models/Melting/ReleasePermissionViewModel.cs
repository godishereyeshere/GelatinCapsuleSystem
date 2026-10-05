using System.ComponentModel.DataAnnotations;

namespace GelatinCapsule.Web.Models.Melting;

public class ReleasePermissionViewModel
{

    public int? Id { get; set; }   // 👈 این خط رو اضافه کن


    // ========== اطلاعات پایه (ورودی کاربر) ==========
    [Required(ErrorMessage = "تاریخ الزامی است")]
    [RegularExpression(@"^\d{4}/\d{2}/\d{2}$", ErrorMessage = "فرمت تاریخ صحیح نیست")]
    [Display(Name = "تاریخ")]
    public string Farsidate { get; set; } = null!;

    [Required(ErrorMessage = "شیفت الزامی است")]
    [Display(Name = "شیفت")]
    public string Shift { get; set; } = "صبح";




    [Required(ErrorMessage = "نوع تولید الزامی است")]
    [Display(Name = "نوع تولید")]
    public string CCode { get; set; } = "Y000";  // Y000 یا W000

    [Required(ErrorMessage = "شماره دستگاه الزامی است")]
    [Range(0, 20, ErrorMessage = "شماره دستگاه بین 0 تا 20 باشد")]
    [Display(Name = "شماره دستگاه")]
    public int Machine { get; set; } = 0;

    // ========== ورودی‌های محاسباتی ==========
    [Required(ErrorMessage = "حجم اولیه الزامی است")]
    [Range(0.1, 2000, ErrorMessage = "حجم اولیه باید بین 0.1 تا 2000 لیتر باشد")]
    [Display(Name = "حجم اولیه (لیتر)")]
    public decimal Vol0 { get; set; } = 943.15M;

    [Required(ErrorMessage = "ویسکوزیته اولیه الزامی است")]
    [Range(1, 10000)]
    [Display(Name = "ویسکوزیته اولیه")]
    public decimal Vis0 { get; set; } = 1000M;

    [Required(ErrorMessage = "ویسکوزیته نهایی الزامی است")]
    [Range(1, 10000)]
    [Display(Name = "ویسکوزیته نهایی")]
    public decimal Vis1 { get; set; } = 1000M;

    [Range(0, 500)]
    [Display(Name = "مدت حباب‌گیری (دقیقه)")]
    public int LagTime { get; set; } = 0;

    // ========== اطلاعات محصول ==========
    [Display(Name = "نام محصول")]
    public string? ProductName { get; set; }

    [Display(Name = "کمپانی")]
    public string? Company { get; set; } = "IGCC";

    [Display(Name = "کد رنگ")]
    public string? ColorCode { get; set; }

    [Display(Name = "شماره تانک")]
    public string? TankNo { get; set; }

    [StringLength(2000)]
    [Display(Name = "توضیحات")]
    public string? Notes { get; set; }

    // ========== محاسبات (فقط خواندنی) ==========
    public decimal? C0 { get; set; }
    public decimal? C1 { get; set; }
    public decimal? Vol1 { get; set; }
    public decimal? Wei0 { get; set; }
    public decimal? EqcCapsule { get; set; }
    public decimal? WtrNeed { get; set; }

    // ========== خودکار تولید شده ==========
    public string? FeedRecNo { get; set; }
    public string? BatchNo { get; set; }
    public int? Serial { get; set; }

    [Display(Name = "شماره ثبت مجوز")]
    public int? LicenseSerial { get; set; }

    [Display(Name = "سریال ملت مربوطه")]
    public int? RelatedMeltId { get; set; }


    // ========== فرمول مواد ==========
    public List<FormulaRowViewModel> FormulaRows { get; set; } = new();

    // ========== Dropdown Options ==========
    public List<CCodeOption> CCodeOptions => new()
    {
        new("Y000", "زرد کهربایی (Y000)"),
        new("W000", "پیست سفید (W000)")
    };

    public List<string> ShiftOptions => new() { "صبح", "شب" };
}

public class FormulaRowViewModel
{
    public string MaterialCode { get; set; } = null!;
    public string MaterialDisplayName { get; set; } = null!;
    public decimal Percentage { get; set; }
    public decimal RequiredGr { get; set; }  // محاسبه شده
    public int SortOrder { get; set; }
}

public record CCodeOption(string Value, string Label);