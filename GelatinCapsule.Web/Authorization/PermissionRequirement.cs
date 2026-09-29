using Microsoft.AspNetCore.Authorization;

namespace GelatinCapsule.Web.Authorization;

public class PermissionRequirement : IAuthorizationRequirement
{
    public string FormCode { get; }
    public string PermissionCode { get; }

    public PermissionRequirement(string formCode, string permissionCode)
    {
        FormCode = formCode;
        PermissionCode = permissionCode;
    }
}