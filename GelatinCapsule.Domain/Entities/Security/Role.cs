using GelatinCapsule.Domain.Common;

namespace GelatinCapsule.Domain.Entities.Security;

public class Role : BaseEntity
{
    public string Name { get; set; } = null!;         // مثل Administrator
    public string DisplayName { get; set; } = null!;  // مثل مدیر سیستم
    public string? Description { get; set; }
    public bool IsSystemRole { get; set; } = false;   // نقش سیستمی قابل حذف نیست
    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<RoleModuleAccess> ModuleAccesses { get; set; } = new List<RoleModuleAccess>();
    public ICollection<RoleFormPermission> FormPermissions { get; set; } = new List<RoleFormPermission>();
    public ICollection<RoleFormDataScope> FormDataScopes { get; set; } = new List<RoleFormDataScope>();
}