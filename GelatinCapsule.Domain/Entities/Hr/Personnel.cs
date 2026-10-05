using GelatinCapsule.Domain.Common;
using GelatinCapsule.Domain.Entities.Security;

namespace GelatinCapsule.Domain.Entities.Hr;

public class Personnel : BaseEntity
{
    public string FullName { get; set; } = null!;       // علی محمدی
    public string? PersonnelCode { get; set; }          // کد پرسنلی
    public string? NationalCode { get; set; }           // کد ملی
    public string? Mobile { get; set; }                 // موبایل

    // ============ اطلاعات شغلی ============
    // می‌تونه چندتا سمت داشته باشه. به صورت متن با کاما ذخیره می‌شه.
    // مثال: "سرشیفت,ملتر"
    public string? Positions { get; set; }

    // ============ واحد سازمانی ============
    public int? DepartmentId { get; set; }
    public Department? Department { get; set; }

    // ============ وضعیت ============
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }
}