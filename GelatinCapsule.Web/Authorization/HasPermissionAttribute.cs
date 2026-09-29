using Microsoft.AspNetCore.Authorization;

namespace GelatinCapsule.Web.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class HasPermissionAttribute : AuthorizeAttribute
{
    public const string PolicyPrefix = "Permission:";

    public HasPermissionAttribute(string formCode, string permissionCode)
    {
        Policy = $"{PolicyPrefix}{formCode}:{permissionCode}";
    }
}