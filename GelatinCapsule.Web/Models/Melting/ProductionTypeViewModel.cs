using System.ComponentModel.DataAnnotations;

namespace GelatinCapsule.Web.Models.Melting;

public class ProductionTypeViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "کد مورد تولید الزامی است")]
    [StringLength(50)]
    [Display(Name = "کد مورد تولید")]
    public string Code { get; set; } = null!;

    [Required(ErrorMessage = "نام نمایشی الزامی است")]
    [StringLength(200)]
    [Display(Name = "نام نمایشی")]
    public string DisplayName { get; set; } = null!;

    [Required(ErrorMessage = "نوع محصول الزامی است")]
    [StringLength(30)]
    [Display(Name = "نوع محصول (PType)")]
    public string PType { get; set; } = null!;

    [Required(ErrorMessage = "کد محصول الزامی است")]
    [StringLength(20)]
    [Display(Name = "کد محصول (CCode)")]
    public string CCode { get; set; } = null!;

    [Required(ErrorMessage = "کد قسمت الزامی است")]
    [Range(1, 99)]
    [Display(Name = "کد قسمت (PartCode)")]
    public int PartCode { get; set; }

    [Required(ErrorMessage = "کد مجوز الزامی است")]
    [Display(Name = "کد مجوز")]
    public int PermissionCode { get; set; }

    [Required(ErrorMessage = "حجم اولیه پیش‌فرض الزامی است")]
    [Range(0.1, 5000)]
    [Display(Name = "حجم اولیه پیش‌فرض (لیتر)")]
    public decimal DefaultVol0 { get; set; }

    [Display(Name = "ترتیب نمایش")]
    public int SortOrder { get; set; }

    [Display(Name = "فعال")]
    public bool IsActive { get; set; } = true;

    [StringLength(500)]
    [Display(Name = "توضیحات")]
    public string? Notes { get; set; }

    // ========== Dropdown CCode ==========
    public List<CCodeOptionDto> AvailableCCodes { get; set; } = new();
}

public class CCodeOptionDto
{
    public string Value { get; set; } = null!;
    public string Label { get; set; } = null!;
}