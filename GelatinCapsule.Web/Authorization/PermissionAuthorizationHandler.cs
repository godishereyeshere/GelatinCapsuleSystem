using GelatinCapsule.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace GelatinCapsule.Web.Authorization;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IPermissionService _permissionService;
    private readonly ICurrentUserService _currentUser;

    public PermissionAuthorizationHandler(
        IPermissionService permissionService,
        ICurrentUserService currentUser)
    {
        _permissionService = permissionService;
        _currentUser = currentUser;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
            return;

        var has = await _permissionService.HasPermissionAsync(
            _currentUser.UserId.Value,
            requirement.FormCode,
            requirement.PermissionCode);

        if (has)
            context.Succeed(requirement);
    }
}