using GelatinCapsule.Domain.Common;

namespace GelatinCapsule.Domain.Entities.Security;

public class Module : BaseEntity
{
    public string Code { get; set; } = null!;         // مثل Production
    public string Name { get; set; } = null!;         // Production
    public string DisplayName { get; set; } = null!;  // تولید
    public string? Icon { get; set; }
    public int SortOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<Form> Forms { get; set; } = new List<Form>();
    public ICollection<UserModuleAccess> UserAccesses { get; set; } = new List<UserModuleAccess>();
    public ICollection<RoleModuleAccess> RoleAccesses { get; set; } = new List<RoleModuleAccess>();
}