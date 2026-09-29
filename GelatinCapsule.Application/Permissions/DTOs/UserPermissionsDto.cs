namespace GelatinCapsule.Application.Features.Permissions.DTOs;

public class UserPermissionsDto
{
    public int UserId { get; set; }
    public HashSet<string> Permissions { get; set; } = new();
    public HashSet<int> AccessibleModuleIds { get; set; } = new();
    public Dictionary<string, byte> DataScopes { get; set; } = new();

    // کلید: "FormCode:PermissionCode" مثل "Users:Edit"
    public bool HasPermission(string formCode, string permissionCode)
    {
        return Permissions.Contains($"{formCode}:{permissionCode}");
    }

    public bool HasModuleAccess(int moduleId)
    {
        return AccessibleModuleIds.Contains(moduleId);
    }

    public byte GetDataScope(string formCode)
    {
        return DataScopes.TryGetValue(formCode, out var scope) ? scope : (byte)0;
    }
}