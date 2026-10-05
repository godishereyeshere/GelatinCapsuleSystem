using System.ComponentModel.DataAnnotations;

namespace GelatinCapsule.Web.Models.Roles;

public class RoleEditViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "نام انگلیسی الزامی است")]
    [StringLength(100)]
    [Display(Name = "نام انگلیسی")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "نام نمایشی الزامی است")]
    [StringLength(200)]
    [Display(Name = "نام نمایشی")]
    public string DisplayName { get; set; } = null!;

    [StringLength(500)]
    [Display(Name = "توضیحات")]
    public string? Description { get; set; }

    [Display(Name = "فعال")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "نقش سیستمی (غیرقابل حذف)")]
    public bool IsSystemRole { get; set; }
}