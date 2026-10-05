using System.ComponentModel.DataAnnotations;

namespace GelatinCapsule.Web.Models.Personnel;

public class PersonnelViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "نام و نام خانوادگی الزامی است")]
    [StringLength(200)]
    [Display(Name = "نام و نام خانوادگی")]
    public string FullName { get; set; } = null!;

    [StringLength(50)]
    [Display(Name = "کد پرسنلی")]
    public string? PersonnelCode { get; set; }

    [StringLength(10, MinimumLength = 10, ErrorMessage = "کد ملی باید ۱۰ رقم باشد")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "کد ملی باید فقط عدد و ۱۰ رقمی باشد")]
    [Display(Name = "کد ملی")]
    public string? NationalCode { get; set; }

    [StringLength(20)]
    [RegularExpression(@"^09\d{9}$", ErrorMessage = "شماره موبایل باید با 09 شروع شود و ۱۱ رقم باشد")]
    [Display(Name = "موبایل")]
    public string? Mobile { get; set; }

    // ============ سمت‌ها ============
    [Display(Name = "سمت‌ها")]
    public List<string> SelectedPositions { get; set; } = new();

    // لیست چک‌باکس‌ها
    public List<PositionItem> AvailablePositions { get; set; } = new();

    // ============ سازمانی ============
    [Display(Name = "واحد سازمانی")]
    public int? DepartmentId { get; set; }

    // ============ وضعیت ============
    [Display(Name = "فعال")]
    public bool IsActive { get; set; } = true;

    [StringLength(1000)]
    [Display(Name = "توضیحات")]
    public string? Notes { get; set; }

    // ============ Dropdown ============
    public List<DepartmentOption> AvailableDepartments { get; set; } = new();
}

public class PositionItem
{
    public string Code { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public bool IsSelected { get; set; }
}

public class DepartmentOption
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}