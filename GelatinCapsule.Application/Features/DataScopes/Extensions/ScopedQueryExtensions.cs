using GelatinCapsule.Application.Features.DataScopes.DTOs;
using GelatinCapsule.Domain.Common;

namespace GelatinCapsule.Application.Features.DataScopes.Extensions;

public static class ScopedQueryExtensions
{
    /// <summary>
    /// فیلترهای DataScope رو روی Query اعمال می‌کنه.
    /// - Admin: همه چیز
    /// - غیر Admin: رکوردهای خودش + رکوردهای واحدش
    /// </summary>
    public static IQueryable<T> ApplyDataScope<T>(
        this IQueryable<T> query,
        DataScopeContext scope)
        where T : IScopedEntity
    {
        // کاربر لاگین نکرده → هیچی نمی‌بینه
        if (!scope.IsAuthenticated)
            return query.Where(e => false);

        // ادمین همه چیز رو می‌بینه
        if (scope.IsAdmin)
            return query;

        var userId = scope.UserId;
        var deptIds = scope.AccessibleDepartmentIds.ToList();

        return query.Where(e =>
            e.CreatedBy == userId ||
            (e.OwnerDepartmentId != null && deptIds.Contains(e.OwnerDepartmentId.Value)));
    }
}