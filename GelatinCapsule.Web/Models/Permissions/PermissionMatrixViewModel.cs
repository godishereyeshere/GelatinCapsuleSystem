namespace GelatinCapsule.Web.Models.Permissions;

public class PermissionMatrixViewModel
{
    public int RoleId { get; set; }
    public string RoleName { get; set; } = null!;
    public string RoleDisplayName { get; set; } = null!;

    // لیست همه‌ی Permission ها (View, Create, Edit, Delete, ...)
    public List<PermissionColumn> PermissionColumns { get; set; } = new();

    // گروه‌بندی بر اساس ماژول
    public List<ModuleGroup> Modules { get; set; } = new();

    // دسترسی ماژول‌ها (Module Access)
    public List<ModuleAccessItem> ModuleAccesses { get; set; } = new();
}

public class PermissionColumn
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
}

public class ModuleGroup
{
    public int ModuleId { get; set; }
    public string ModuleDisplayName { get; set; } = null!;
    public List<FormRow> Forms { get; set; } = new();
}

public class FormRow
{
    public int FormId { get; set; }
    public string FormCode { get; set; } = null!;
    public string FormDisplayName { get; set; } = null!;

    // PermissionId هایی که این نقش داره
    public HashSet<int> GrantedPermissionIds { get; set; } = new();
}

public class ModuleAccessItem
{
    public int ModuleId { get; set; }
    public string ModuleDisplayName { get; set; } = null!;
    public bool HasAccess { get; set; }
}