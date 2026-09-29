using GelatinCapsule.Application.Features.Permissions.DTOs;

namespace GelatinCapsule.Application.Common.Interfaces;

public interface IPermissionService
{
    Task<UserPermissionsDto> GetUserPermissionsAsync(int userId, CancellationToken ct = default);
    Task<bool> HasPermissionAsync(int userId, string formCode, string permissionCode, CancellationToken ct = default);
    Task<bool> HasModuleAccessAsync(int userId, int moduleId, CancellationToken ct = default);
    Task InvalidateCacheAsync(int userId);
}