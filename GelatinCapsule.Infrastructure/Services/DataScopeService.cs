using GelatinCapsule.Application.Common.Interfaces;
using GelatinCapsule.Application.Features.DataScopes.DTOs;
using GelatinCapsule.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace GelatinCapsule.Infrastructure.Services;

public class DataScopeService : IDataScopeService
{
    private const string CacheKeyPrefix = "DataScope_";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);
    private const string AdministratorRoleName = "Administrator";

    private readonly ApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IMemoryCache _cache;

    public DataScopeService(
        ApplicationDbContext db,
        ICurrentUserService currentUser,
        IMemoryCache cache)
    {
        _db = db;
        _currentUser = currentUser;
        _cache = cache;
    }

    public async Task<DataScopeContext> GetCurrentScopeAsync(CancellationToken ct = default)
    {
        var ctx = new DataScopeContext();

        if (!_currentUser.IsAuthenticated || _currentUser.UserId == null)
            return ctx;

        var userId = _currentUser.UserId.Value;
        var cacheKey = $"{CacheKeyPrefix}{userId}";

        if (_cache.TryGetValue(cacheKey, out DataScopeContext? cached) && cached != null)
            return cached;

        var user = await _db.Users
            .IgnoreQueryFilters()
            .Where(u => u.Id == userId)
            .Select(u => new { u.Id, u.DepartmentId })
            .FirstOrDefaultAsync(ct);

        if (user == null)
            return ctx;

        ctx.UserId = user.Id;
        ctx.UserDepartmentId = user.DepartmentId;

        // با Join صریح — مطمئن‌تر از navigation property
        ctx.IsAdmin = await (
            from ur in _db.UserRoles
            join r in _db.Roles on ur.RoleId equals r.Id
            where ur.UserId == userId
               && r.Name == AdministratorRoleName
               && !r.IsDeleted
            select ur
        ).AnyAsync(ct);

        if (ctx.IsAdmin)
        {
            var deptIds = await _db.Departments
                .Where(d => !d.IsDeleted)
                .Select(d => d.Id)
                .ToListAsync(ct);

            ctx.AccessibleDepartmentIds = deptIds.ToHashSet();
        }
        else if (user.DepartmentId.HasValue)
        {
            var allDepts = await _db.Departments
                .Where(d => !d.IsDeleted)
                .Select(d => new DeptInfo { Id = d.Id, ParentId = d.ParentId })
                .ToListAsync(ct);

            ctx.AccessibleDepartmentIds = GetAllDescendants(user.DepartmentId.Value, allDepts);
        }

        _cache.Set(cacheKey, ctx, CacheDuration);
        return ctx;
    }

    public Task InvalidateCacheAsync(int userId)
    {
        _cache.Remove($"{CacheKeyPrefix}{userId}");
        return Task.CompletedTask;
    }

    private static HashSet<int> GetAllDescendants(int rootId, List<DeptInfo> allDepts)
    {
        var result = new HashSet<int> { rootId };
        var queue = new Queue<int>();
        queue.Enqueue(rootId);

        while (queue.Count > 0)
        {
            var currentId = queue.Dequeue();

            var children = allDepts
                .Where(d => d.ParentId == currentId)
                .Select(d => d.Id)
                .ToList();

            foreach (var childId in children)
            {
                if (result.Add(childId))
                    queue.Enqueue(childId);
            }
        }

        return result;
    }

    private class DeptInfo
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
    }
}