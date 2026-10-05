using System.ComponentModel.DataAnnotations;

namespace GelatinCapsule.Web.Models.Melting;

public class ShiftInfoViewModel
{
    public int? Id { get; set; }

    // ============ اطلاعات پایه ============
    [Required(ErrorMessage = "تاریخ الزامی است")]
    [StringLength(10, MinimumLength = 10, ErrorMessage = "تاریخ باید به فرمت 1405/05/06 باشد")]
    [RegularExpression(@"^\d{4}/\d{2}/\d{2}$", ErrorMessage = "فرمت تاریخ صحیح نیست")]
    [Display(Name = "تاریخ (شمسی)")]
    public string Farsidate { get; set; } = null!;

    [Required(ErrorMessage = "شیفت الزامی است")]
    [Display(Name = "شیفت")]
    public string Shift { get; set; } = "صبح";

    [Display(Name = "گروه")]
    public string? Group { get; set; }

    // ============ پرسنل اصلی ============
    [Display(Name = "سوپروایزر")]
    public string? Supervisore { get; set; }

    [Display(Name = "سرشیفت")]
    public string? SheftHeader { get; set; }

    [Display(Name = "ملتر ۱")]
    public string? GelMaker1 { get; set; }

    [Display(Name = "ملتر ۲")]
    public string? GelMaker2 { get; set; }

    [Display(Name = "ناظر کنترل کیفی")]
    public string? IpQc { get; set; }

    [Display(Name = "شوینده تانک ۱")]
    public string? TankWasher { get; set; }

    [Display(Name = "شوینده تانک ۲")]
    public string? TankWasher2 { get; set; }

    [Display(Name = "ملتر کمکی")]
    public string? HelpMelter { get; set; }

    // ============ جانشین ها ============
    [Display(Name = "جانشین سرشیفت")]
    public string? SheftHeaderRep { get; set; }

    [Display(Name = "سرشیفت جانشین از ساعت")]
    public string? SheftHeaderRepFrom { get; set; }

    [Display(Name = "جانشین ملتر ۱")]
    public string? GelMaker1Rep { get; set; }

    [Display(Name = "ملتر ۱ جانشین از ساعت")]
    public string? GelMaker1RepFrom { get; set; }

    [Display(Name = "جانشین ملتر ۲")]
    public string? GelMaker2Rep { get; set; }

    [Display(Name = "ملتر ۲ جانشین از ساعت")]
    public string? GelMaker2RepFrom { get; set; }

    [Display(Name = "جانشین شوینده تانک")]
    public string? TankWasherRep { get; set; }

    [Display(Name = "شوینده جانشین از ساعت")]
    public string? TankWasherRepFrom { get; set; }

    // ============ Helper ============
    public List<string> ShiftOptions => new() { "صبح", "شب" };
    public List<string> GroupOptions => new() { "A", "B", "C", "D" };
}