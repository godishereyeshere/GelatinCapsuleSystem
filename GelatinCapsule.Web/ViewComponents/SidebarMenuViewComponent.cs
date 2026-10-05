using GelatinCapsule.Application.Common.Interfaces;
using GelatinCapsule.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GelatinCapsule.Web.ViewComponents;

public class SidebarMenuViewComponent : ViewComponent
{
    private readonly ApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IPermissionService _permissionService;

    public SidebarMenuViewComponent(
        ApplicationDbContext db,
        ICurrentUserService currentUser,
        IPermissionService permissionService)
    {
        _db = db;
        _currentUser = currentUser;
        _permissionService = permissionService;
    }

    public async Task<IViewComponentResult> InvokeAsync(CancellationToken ct = default)
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId == null)
            return Content(string.Empty);

        // دسترسی‌های کاربر
        var userPerms = await _permissionService.GetUserPermissionsAsync(_currentUser.UserId.Value, ct);

        // همه‌ی ماژول‌ها + فرم‌هاشون
        var modules = await _db.Modules
            .Where(m => m.IsActive)
            .OrderBy(m => m.SortOrder)
            .Include(m => m.Forms.Where(f => f.IsActive && f.ShowInMenu).OrderBy(f => f.SortOrder))
            .ToListAsync(ct);

        // فیلتر: فقط ماژول‌هایی که کاربر دسترسی داره
        var accessibleModules = modules
            .Where(m => userPerms.HasModuleAccess(m.Id))
            .Select(m => new
            {
                m.DisplayName,
                m.Icon,
                Forms = m.Forms
                    .Where(f => userPerms.HasPermission(f.Code, "View"))
                    .Select(f => new
                    {
                        f.Code,
                        f.DisplayName,
                        f.ControllerName,
                        f.ActionName
                    })
                    .ToList()
            })
            .Where(m => m.Forms.Any())
            .ToList();

        return View(accessibleModules);
    }
}