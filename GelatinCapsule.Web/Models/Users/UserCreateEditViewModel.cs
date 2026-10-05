using System.ComponentModel.DataAnnotations;

namespace GelatinCapsule.Web.Models.Users;

public class UserCreateEditViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "نام کاربری الزامی است")]
    [StringLength(100)]
    [Display(Name = "نام کاربری")]
    public string Username { get; set; } = null!;

    [StringLength(100, MinimumLength = 6, ErrorMessage = "رمز عبور باید حداقل ۶ کاراکتر باشد")]
    [DataType(DataType.Password)]
    [Display(Name = "رمز عبور")]
    public string? Password { get; set; }

    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "رمز عبور و تکرار آن یکسان نیستند")]
    [Display(Name = "تکرار رمز عبور")]
    public string? ConfirmPassword { get; set; }

    [Required(ErrorMessage = "نام کامل الزامی است")]
    [StringLength(200)]
    [Display(Name = "نام کامل")]
    public string FullName { get; set; } = null!;

    [StringLength(50)]
    [Display(Name = "کد پرسنلی")]
    public string? PersonnelCode { get; set; }

    [StringLength(10)]
    [Display(Name = "کد ملی")]
    public string? NationalCode { get; set; }

    [EmailAddress(ErrorMessage = "ایمیل معتبر نیست")]
    [StringLength(200)]
    [Display(Name = "ایمیل")]
    public string? Email { get; set; }

    [StringLength(20)]
    [Display(Name = "موبایل")]
    public string? Mobile { get; set; }

    [Display(Name = "واحد سازمانی")]
    public int? DepartmentId { get; set; }

    [Display(Name = "فعال")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "قفل شده")]
    public bool IsLockedOut { get; set; }

    [Display(Name = "مجبور به تغییر رمز در ورود بعدی")]
    public bool MustChangePassword { get; set; }

    // ============ نقش‌ها ============
    [Display(Name = "نقش‌ها")]
    public List<int> SelectedRoleIds { get; set; } = new();

    // لیست برای نمایش در UI
    public List<RoleCheckboxItem> AvailableRoles { get; set; } = new();
    public List<DepartmentSelectItem> AvailableDepartments { get; set; } = new();
}

public class RoleCheckboxItem
{
    public int Id { get; set; }
    public string DisplayName { get; set; } = null!;
    public bool IsSelected { get; set; }
}

public class DepartmentSelectItem
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}