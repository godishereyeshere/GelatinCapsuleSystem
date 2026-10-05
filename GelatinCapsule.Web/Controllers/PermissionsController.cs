using GelatinCapsule.Domain.Entities.Security;
using GelatinCapsule.Infrastructure.Persistence;
using GelatinCapsule.Web.Authorization;
using GelatinCapsule.Web.Models.Permissions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GelatinCapsule.Web.Controllers;

[HasPermission("Permissions", "View")]
public class PermissionsController : Controller
{
    private readonly ApplicationDbContext _db;

    public PermissionsController(ApplicationDbContext db)
    {
        _db = db;
    }

    // ==========================================
    // صفحه‌ی اصلی: انتخاب نقش
    // ==========================================
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var roles = await _db.Roles
            .Where(r => r.IsActive)
            .OrderByDescending(r => r.IsSystemRole)
            .ThenBy(r => r.DisplayName)
            .ToListAsync(ct);

        return View(roles);
    }

    // ==========================================
    // ماتریس مجوزها برای یک نقش
    // ==========================================
    [HasPermission("Permissions", "Edit")]
    [HttpGet]
    public async Task<IActionResult> Matrix(int roleId, CancellationToken ct)
    {
        var role = await _db.Roles.FirstOrDefaultAsync(r => r.Id == roleId, ct);
        if (role == null) return NotFound();

        var vm = new PermissionMatrixViewModel
        {
            RoleId = role.Id,
            RoleName = role.Name,
            RoleDisplayName = role.DisplayName
        };

        // ۱) همه‌ی Permission ها (ستون‌های جدول)
        vm.PermissionColumns = await _db.Permissions
            .OrderBy(p => p.SortOrder)
            .Select(p => new PermissionColumn
            {
                Id = p.Id,
                Code = p.Code,
                DisplayName = p.DisplayName
            })
            .ToListAsync(ct);

        // ۲) همه‌ی ماژول‌ها با فرم‌هاشون
        var modules = await _db.Modules
            .Where(m => m.IsActive)
            .OrderBy(m => m.SortOrder)
            .Include(m => m.Forms.Where(f => f.IsActive).OrderBy(f => f.SortOrder))
            .ToListAsync(ct);

        // ۳) دسترسی‌های فعلی این نقش
        var roleFormPerms = await _db.RoleFormPermissions
            .Where(rfp => rfp.RoleId == roleId)
            .ToListAsync(ct);

        var roleModuleAccesses = await _db.RoleModuleAccesses
            .Where(rma => rma.RoleId == roleId)
            .Select(rma => rma.ModuleId)
            .ToListAsync(ct);

        // ۴) ساخت ساختار
        foreach (var module in modules)
        {
            var group = new ModuleGroup
            {
                ModuleId = module.Id,
                ModuleDisplayName = module.DisplayName
            };

            foreach (var form in module.Forms)
            {
                var row = new FormRow
                {
                    FormId = form.Id,
                    FormCode = form.Code,
                    FormDisplayName = form.DisplayName,
                    GrantedPermissionIds = roleFormPerms
                        .Where(rfp => rfp.FormId == form.Id)
                        .Select(rfp => rfp.PermissionId)
                        .ToHashSet()
                };
                group.Forms.Add(row);
            }

            if (group.Forms.Any())
                vm.Modules.Add(group);

            // ماژول اکسس
            vm.ModuleAccesses.Add(new ModuleAccessItem
            {
                ModuleId = module.Id,
                ModuleDisplayName = module.DisplayName,
                HasAccess = roleModuleAccesses.Contains(module.Id)
            });
        }

        return View(vm);
    }

    // ==========================================
    // ذخیره‌ی ماتریس
    // ==========================================
    [HasPermission("Permissions", "Edit")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Matrix(PermissionMatrixViewModel vm, CancellationToken ct)
    {
        var role = await _db.Roles.FirstOrDefaultAsync(r => r.Id == vm.RoleId, ct);
        if (role == null) return NotFound();

        // ⚠️ نکته: چون PermissionColumns توی Form POST خالی میاد (فقط Id های چک‌شده میاد)
        // از Request.Form مستقیم می‌خونیم

        // ۱) پاک کردن دسترسی‌های قبلی فرم‌ها
        var existingFormPerms = await _db.RoleFormPermissions
            .Where(rfp => rfp.RoleId == vm.RoleId)
            .ToListAsync(ct);
        _db.RoleFormPermissions.RemoveRange(existingFormPerms);

        // ۲) پاک کردن دسترسی‌های قبلی ماژول‌ها
        var existingModuleAccesses = await _db.RoleModuleAccesses
            .Where(rma => rma.RoleId == vm.RoleId)
            .ToListAsync(ct);
        _db.RoleModuleAccesses.RemoveRange(existingModuleAccesses);

        // ۳) خوندن چک‌باکس‌های تیک‌خورده از Request
        // فرمت کلید: perm_{formId}_{permissionId}
        var form = Request.Form;
        var allForms = await _db.Forms.Where(f => f.IsActive).ToListAsync(ct);

        foreach (var formEntity in allForms)
        {
            var permIdsForThisForm = form.Keys
                .Where(k => k.StartsWith($"perm_{formEntity.Id}_"))
                .Select(k => k.Split('_').Last())
                .Where(v => int.TryParse(v, out _))
                .Select(int.Parse)
                .Distinct()
                .ToList();

            foreach (var permId in permIdsForThisForm)
            {
                _db.RoleFormPermissions.Add(new RoleFormPermission
                {
                    RoleId = vm.RoleId,
                    FormId = formEntity.Id,
                    PermissionId = permId
                });
            }
        }

        // ۴) خوندن ماژول اکسس‌ها
        // فرمت کلید: module_{moduleId}
        var allModules = await _db.Modules.Where(m => m.IsActive).ToListAsync(ct);
        foreach (var module in allModules)
        {
            if (form.ContainsKey($"module_{module.Id}"))
            {
                _db.RoleModuleAccesses.Add(new RoleModuleAccess
                {
                    RoleId = vm.RoleId,
                    ModuleId = module.Id
                });
            }
        }

        await _db.SaveChangesAsync(ct);

        // پاک کردن کش مجوزهای همه‌ی کاربرانی که این نقش رو دارن
        // (کش در PermissionService با IMemoryCache هست، ولی چون Scoped نیست نمی‌تونیم اینجا پاک کنیم.
        //  در قدم بعدی با یه سرویس Invalidation اینو حل می‌کنیم)
        TempData["Success"] = $"دسترسی‌های نقش «{role.DisplayName}» با موفقیت ذخیره شد";
        return RedirectToAction(nameof(Matrix), new { roleId = vm.RoleId });
    }
}