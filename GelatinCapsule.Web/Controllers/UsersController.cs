using GelatinCapsule.Domain.Entities.Security;
using GelatinCapsule.Infrastructure.Persistence;
using GelatinCapsule.Infrastructure.Services;
using GelatinCapsule.Web.Authorization;
using GelatinCapsule.Web.Models.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using GelatinCapsule.Application.Features.DataScopes.Extensions;
using Microsoft.EntityFrameworkCore;
using GelatinCapsule.Application.Common.Interfaces;
using GelatinCapsule.Domain.Enums;

namespace GelatinCapsule.Web.Controllers;

[HasPermission("Users", "View")]
public class UsersController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly PasswordHasher _hasher;
    private readonly IDataScopeService _dataScope;
    private readonly ICurrentUserService _currentUser;

    public UsersController(ApplicationDbContext db, PasswordHasher hasher, IDataScopeService dataScope, ICurrentUserService currentUser)
    {
        _db = db;
        _hasher = hasher;
        _dataScope = dataScope;
        _currentUser = currentUser;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var scope = await _dataScope.GetCurrentScopeAsync(ct);

        var users = await _db.Users
            .Include(u => u.Department)
            .ApplyDataScope(scope)                     // 👈 فیلتر خودکار
            .OrderBy(u => u.Username)
            .ToListAsync(ct);

        return View(users);
    }

    // ============ CREATE ============
    [HasPermission("Users", "Create")]
    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        var vm = new UserCreateEditViewModel();
        await PopulateDropdownsAsync(vm, ct);
        return View(vm);
    }

    [HasPermission("Users", "Create")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserCreateEditViewModel vm, CancellationToken ct)
    {
        // چک یکتایی نام کاربری
        if (await _db.Users.AnyAsync(u => u.Username == vm.Username, ct))
            ModelState.AddModelError(nameof(vm.Username), "این نام کاربری قبلاً استفاده شده است");

        if (string.IsNullOrWhiteSpace(vm.Password))
            ModelState.AddModelError(nameof(vm.Password), "رمز عبور الزامی است");

        if (!ModelState.IsValid)
        {
            await PopulateDropdownsAsync(vm, ct);
            return View(vm);
        }

        var user = new User
        {
            Username = vm.Username.Trim(),
            PasswordHash = _hasher.Hash(vm.Password!),
            FullName = vm.FullName.Trim(),
            PersonnelCode = vm.PersonnelCode?.Trim(),
            NationalCode = vm.NationalCode?.Trim(),
            Email = vm.Email?.Trim(),
            Mobile = vm.Mobile?.Trim(),
            DepartmentId = vm.DepartmentId,
            OwnerDepartmentId = vm.DepartmentId,
            CreatedBy = _currentUser.UserId,          // 👈 جدید
            IsActive = vm.IsActive,
            IsLockedOut = vm.IsLockedOut,
            MustChangePassword = vm.MustChangePassword
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);

        // ذخیره نقش‌ها
        if (vm.SelectedRoleIds.Any())
        {
            foreach (var roleId in vm.SelectedRoleIds)
            {
                _db.UserRoles.Add(new UserRole
                {
                    UserId = user.Id,
                    RoleId = roleId
                });
            }
            await _db.SaveChangesAsync(ct);
        }

        TempData["Success"] = "کاربر با موفقیت ایجاد شد";
        return RedirectToAction(nameof(Index));
    }

    // ============ EDIT ============
    [HasPermission("Users", "Edit")]
    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var user = await _db.Users
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == id, ct);

        if (user == null)
            return NotFound();

        var vm = new UserCreateEditViewModel
        {
            Id = user.Id,
            Username = user.Username,
            FullName = user.FullName,
            PersonnelCode = user.PersonnelCode,
            NationalCode = user.NationalCode,
            Email = user.Email,
            Mobile = user.Mobile,
            DepartmentId = user.DepartmentId,
            IsActive = user.IsActive,
            IsLockedOut = user.IsLockedOut,
            MustChangePassword = user.MustChangePassword,
            SelectedRoleIds = user.UserRoles.Select(ur => ur.RoleId).ToList()
        };

        await PopulateDropdownsAsync(vm, ct);
        return View(vm);
    }

    [HasPermission("Users", "Edit")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UserCreateEditViewModel vm, CancellationToken ct)
    {
        if (vm.Id == null)
            return BadRequest();

        var user = await _db.Users
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == vm.Id.Value, ct);

        if (user == null)
            return NotFound();

        // چک یکتایی نام کاربری (به جز خود کاربر)
        if (await _db.Users.AnyAsync(u => u.Username == vm.Username && u.Id != user.Id, ct))
            ModelState.AddModelError(nameof(vm.Username), "این نام کاربری قبلاً استفاده شده است");

        if (!ModelState.IsValid)
        {
            await PopulateDropdownsAsync(vm, ct);
            return View(vm);
        }

        user.Username = vm.Username.Trim();
        user.FullName = vm.FullName.Trim();
        user.PersonnelCode = vm.PersonnelCode?.Trim();
        user.NationalCode = vm.NationalCode?.Trim();
        user.Email = vm.Email?.Trim();
        user.Mobile = vm.Mobile?.Trim();
        user.DepartmentId = vm.DepartmentId;
        user.IsActive = vm.IsActive;
        user.IsLockedOut = vm.IsLockedOut;
        user.MustChangePassword = vm.MustChangePassword;
        user.UpdatedAt = DateTime.UtcNow;

        // اگه رمز جدید داده شده
        if (!string.IsNullOrWhiteSpace(vm.Password))
        {
            user.PasswordHash = _hasher.Hash(vm.Password);
        }

        // آپدیت نقش‌ها — اول حذف، بعد اضافه
        var currentRoleIds = user.UserRoles.Select(ur => ur.RoleId).ToList();
        var newRoleIds = vm.SelectedRoleIds;

        // حذف نقش‌های حذف‌شده
        var toRemove = user.UserRoles.Where(ur => !newRoleIds.Contains(ur.RoleId)).ToList();
        _db.UserRoles.RemoveRange(toRemove);

        // اضافه کردن نقش‌های جدید
        var toAdd = newRoleIds.Where(rid => !currentRoleIds.Contains(rid)).ToList();
        foreach (var rid in toAdd)
        {
            _db.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = rid });
        }

        await _db.SaveChangesAsync(ct);

        TempData["Success"] = "کاربر با موفقیت ویرایش شد";
        return RedirectToAction(nameof(Index));
    }

    // ============ Helper ============
    private async Task PopulateDropdownsAsync(UserCreateEditViewModel vm, CancellationToken ct)
    {
        var roles = await _db.Roles
            .Where(r => r.IsActive)
            .OrderBy(r => r.DisplayName)
            .Select(r => new RoleCheckboxItem
            {
                Id = r.Id,
                DisplayName = r.DisplayName,
                IsSelected = vm.SelectedRoleIds.Contains(r.Id)
            })
            .ToListAsync(ct);

        vm.AvailableRoles = roles;

        vm.AvailableDepartments = await _db.Departments
            .Where(d => d.IsActive)
            .OrderBy(d => d.Name)
            .Select(d => new DepartmentSelectItem
            {
                Id = d.Id,
                Name = d.Name
            })
            .ToListAsync(ct);
    }
}