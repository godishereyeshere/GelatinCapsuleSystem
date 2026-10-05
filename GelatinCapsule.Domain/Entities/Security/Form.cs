using GelatinCapsule.Domain.Common;

namespace GelatinCapsule.Domain.Entities.Security;

public class Form : BaseEntity
{
    public int ModuleId { get; set; }
    public Module Module { get; set; } = null!;

    public string Code { get; set; } = null!;              // RawMaterialEntry
    public string DisplayName { get; set; } = null!;       // ورود مواد اولیه
    public string ControllerName { get; set; } = null!;    // RawMaterial
    public string? ActionName { get; set; }
    public string? MenuPath { get; set; }
    public bool ShowInMenu { get; set; } = true;
    public bool SupportsDataScope { get; set; } = false;   // آیا Row Level Security داره؟
    public int SortOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<RoleFormPermission> RolePermissions { get; set; } = new List<RoleFormPermission>();
    public ICollection<UserFormPermission> UserPermissions { get; set; } = new List<UserFormPermission>();
    public ICollection<RoleFormDataScope> RoleDataScopes { get; set; } = new List<RoleFormDataScope>();
    public ICollection<UserFormDataScope> UserDataScopes { get; set; } = new List<UserFormDataScope>();
}