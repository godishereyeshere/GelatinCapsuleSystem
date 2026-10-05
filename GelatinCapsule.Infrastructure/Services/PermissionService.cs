using GelatinCapsule.Application.Common.Interfaces;
using GelatinCapsule.Application.Features.Permissions.DTOs;
using GelatinCapsule.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace GelatinCapsule.Infrastructure.Services;

public class PermissionService : IPermissionService
{
    private const string CacheKeyPrefix = "UserPerms_";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    private readonly ApplicationDbContext _db;
    private readonly IMemoryCache _cache;

    public PermissionService(ApplicationDbContext db, IMemoryCache cache)
    {
        _db = db;
        _cache = cache;
    }

    public async Task<UserPermissionsDto> GetUserPermissionsAsync(int userId, CancellationToken ct = default)
    {
        var cacheKey = $"{CacheKeyPrefix}{userId}";
        if (_cache.TryGetValue(cacheKey, out UserPermissionsDto? cached) && cached != null)
            return cached;

        var dto = new UserPermissionsDto { UserId = userId };

        // 1) نقش‌های کاربر
        var roleIds = await _db.UserRoles
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.RoleId)
            .ToListAsync(ct);

        // 2) دسترسی مستقیم کاربر به ماژول‌ها
        var userModuleIds = await _db.UserModuleAccesses
            .Where(uma => uma.UserId == userId)
            .Select(uma => uma.ModuleId)
            .ToListAsync(ct);

        // 3) دسترسی نقش‌ها به ماژول‌ها
        var roleModuleIds = await _db.RoleModuleAccesses
            .Where(rma => roleIds.Contains(rma.RoleId))
            .Select(rma => rma.ModuleId)
            .ToListAsync(ct);

        foreach (var mid in userModuleIds.Concat(roleModuleIds).Distinct())
            dto.AccessibleModuleIds.Add(mid);

        // 4) دسترسی مستقیم کاربر به فرم‌ها
        var userPerms = await _db.UserFormPermissions
            .Where(ufp => ufp.UserId == userId)
            .Include(ufp => ufp.Form)
            .Include(ufp => ufp.Permission)
            .ToListAsync(ct);

        // اول Deny ها رو بردار (خالی کن)، بعد Grant ها رو اضافه کن
        foreach (var up in userPerms.Where(x => !x.IsGranted))
            dto.Permissions.Remove($"{up.Form.Code}:{up.Permission.Code}");

        foreach (var up in userPerms.Where(x => x.IsGranted))
            dto.Permissions.Add($"{up.Form.Code}:{up.Permission.Code}");

        // 5) دسترسی نقش‌ها به فرم‌ها
        var rolePerms = await _db.RoleFormPermissions
            .Where(rfp => roleIds.Contains(rfp.RoleId))
            .Include(rfp => rfp.Form)
            .Include(rfp => rfp.Permission)
            .ToListAsync(ct);

        foreach (var rp in rolePerms)
            dto.Permissions.Add($"{rp.Form.Code}:{rp.Permission.Code}");

        // 6) Data Scope — MAX بین کاربر و نقش‌ها
        var userScopes = await _db.UserFormDataScopes
            .Where(ufds => ufds.UserId == userId)
            .Include(ufds => ufds.Form)
            .ToListAsync(ct);

        foreach (var us in userScopes)
            dto.DataScopes[us.Form.Code] = (byte)us.Scope;

        var roleScopes = await _db.RoleFormDataScopes
            .Where(rfds => roleIds.Contains(rfds.RoleId))
            .Include(rfds => rfds.Form)
            .ToListAsync(ct);

        foreach (var rs in roleScopes)
        {
            var code = rs.Form.Code;
            var scope = (byte)rs.Scope;
            if (!dto.DataScopes.ContainsKey(code) || dto.DataScopes[code] < scope)
                dto.DataScopes[code] = scope;
        }

        _cache.Set(cacheKey, dto, CacheDuration);
        return dto;
    }

    public async Task<bool> HasPermissionAsync(int userId, string formCode, string permissionCode, CancellationToken ct = default)
    {
        var perms = await GetUserPermissionsAsync(userId, ct);
        return perms.HasPermission(formCode, permissionCode);
    }

    public async Task<bool> HasModuleAccessAsync(int userId, int moduleId, CancellationToken ct = default)
    {
        var perms = await GetUserPermissionsAsync(userId, ct);
        return perms.HasModuleAccess(moduleId);
    }

    public Task InvalidateCacheAsync(int userId)
    {
        _cache.Remove($"{CacheKeyPrefix}{userId}");
        return Task.CompletedTask;
    }
}