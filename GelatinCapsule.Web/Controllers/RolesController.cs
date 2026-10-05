using GelatinCapsule.Domain.Entities.Security;
using GelatinCapsule.Infrastructure.Persistence;
using GelatinCapsule.Web.Authorization;
using GelatinCapsule.Web.Models.Roles;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GelatinCapsule.Web.Controllers;

[HasPermission("Roles", "View")]
public class RolesController : Controller
{
    private readonly ApplicationDbContext _db;

    public RolesController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var roles = await _db.Roles
            .OrderByDescending(r => r.IsSystemRole)
            .ThenBy(r => r.DisplayName)
            .ToListAsync(ct);

        // آمار کاربران هر نقش
        var roleUserCounts = await _db.UserRoles
            .GroupBy(ur => ur.RoleId)
            .Select(g => new { RoleId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.RoleId, x => x.Count, ct);

        ViewBag.RoleUserCounts = roleUserCounts;

        return View(roles);
    }

    // ============ CREATE ============
    [HasPermission("Roles", "Create")]
    [HttpGet]
    public IActionResult Create()
    {
        return View(new RoleEditViewModel());
    }

    [HasPermission("Roles", "Create")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RoleEditViewModel vm, CancellationToken ct)
    {
        if (await _db.Roles.AnyAsync(r => r.Name == vm.Name, ct))
            ModelState.AddModelError(nameof(vm.Name), "این نام انگلیسی قبلاً استفاده شده است");

        if (!ModelState.IsValid)
            return View(vm);

        var role = new Role
        {
            Name = vm.Name.Trim(),
            DisplayName = vm.DisplayName.Trim(),
            Description = vm.Description?.Trim(),
            IsActive = vm.IsActive,
            IsSystemRole = false   // از UI قابل تنظیم نیست
        };

        _db.Roles.Add(role);
        await _db.SaveChangesAsync(ct);

        TempData["Success"] = "نقش با موفقیت ایجاد شد";
        return RedirectToAction(nameof(Index));
    }

    // ============ EDIT ============
    [HasPermission("Roles", "Edit")]
    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var role = await _db.Roles.FirstOrDefaultAsync(r => r.Id == id, ct);
        if (role == null) return NotFound();

        var vm = new RoleEditViewModel
        {
            Id = role.Id,
            Name = role.Name,
            DisplayName = role.DisplayName,
            Description = role.Description,
            IsActive = role.IsActive,
            IsSystemRole = role.IsSystemRole
        };

        return View(vm);
    }

    [HasPermission("Roles", "Edit")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(RoleEditViewModel vm, CancellationToken ct)
    {
        if (vm.Id == null) return BadRequest();

        var role = await _db.Roles.FirstOrDefaultAsync(r => r.Id == vm.Id.Value, ct);
        if (role == null) return NotFound();

        if (await _db.Roles.AnyAsync(r => r.Name == vm.Name && r.Id != role.Id, ct))
            ModelState.AddModelError(nameof(vm.Name), "این نام انگلیسی قبلاً استفاده شده است");

        if (!ModelState.IsValid)
            return View(vm);

        // نام انگلیسی نقش‌های سیستمی قابل تغییر نیست
        if (!role.IsSystemRole)
            role.Name = vm.Name.Trim();

        role.DisplayName = vm.DisplayName.Trim();
        role.Description = vm.Description?.Trim();
        role.IsActive = vm.IsActive;
        role.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);

        TempData["Success"] = "نقش با موفقیت ویرایش شد";
        return RedirectToAction(nameof(Index));
    }

    // ============ DELETE ============
    [HasPermission("Roles", "Delete")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var role = await _db.Roles.FirstOrDefaultAsync(r => r.Id == id, ct);
        if (role == null) return NotFound();

        if (role.IsSystemRole)
        {
            TempData["Error"] = "نقش‌های سیستمی قابل حذف نیستند";
            return RedirectToAction(nameof(Index));
        }

        // چک کن کاربری به این نقش وصله
        var hasUsers = await _db.UserRoles.AnyAsync(ur => ur.RoleId == id, ct);
        if (hasUsers)
        {
            TempData["Error"] = "این نقش به کاربران متصل است. ابتدا کاربران را از این نقش خارج کنید";
            return RedirectToAction(nameof(Index));
        }

        role.IsDeleted = true;
        role.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);

        TempData["Success"] = "نقش با موفقیت حذف شد";
        return RedirectToAction(nameof(Index));
    }
}