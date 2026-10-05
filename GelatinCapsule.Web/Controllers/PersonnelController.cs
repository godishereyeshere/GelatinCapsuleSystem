using GelatinCapsule.Domain.Entities.Hr;
using GelatinCapsule.Infrastructure.Persistence;
using GelatinCapsule.Web.Authorization;
using GelatinCapsule.Web.Models.Personnel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GelatinCapsule.Web.Controllers;

[HasPermission("Personnel", "View")]
public class PersonnelController : Controller
{
    private readonly ApplicationDbContext _db;

    public PersonnelController(ApplicationDbContext db)
    {
        _db = db;
    }

    // ==========================================
    // لیست پرسنل
    // ==========================================
    public async Task<IActionResult> Index(
        string? search,
        bool? activeOnly,
        CancellationToken ct)
    {
        var query = _db.Personnel
            .Include(p => p.Department)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p =>
                p.FullName.Contains(search) ||
                (p.PersonnelCode != null && p.PersonnelCode.Contains(search)) ||
                (p.NationalCode != null && p.NationalCode.Contains(search)) ||
                (p.Mobile != null && p.Mobile.Contains(search)));
        }

        if (activeOnly == true)
            query = query.Where(p => p.IsActive);

        var items = await query
            .OrderBy(p => p.FullName)
            .ToListAsync(ct);

        ViewBag.Search = search;
        ViewBag.ActiveOnly = activeOnly;
        ViewBag.TotalCount = await _db.Personnel.CountAsync(ct);
        ViewBag.ActiveCount = await _db.Personnel.CountAsync(p => p.IsActive, ct);

        return View(items);
    }

    // ==========================================
    // جزئیات
    // ==========================================
    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var item = await _db.Personnel
            .Include(p => p.Department)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        if (item == null) return NotFound();
        return View(item);
    }

    // ==========================================
    // ساخت جدید
    // ==========================================
    [HasPermission("Personnel", "Create")]
    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        var vm = new PersonnelViewModel();
        await PopulateDropdownsAsync(vm, ct);
        return View(vm);
    }

    [HasPermission("Personnel", "Create")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PersonnelViewModel vm, CancellationToken ct)
    {
        // چک یکتایی کد پرسنلی
        if (!string.IsNullOrWhiteSpace(vm.PersonnelCode) &&
            await _db.Personnel.AnyAsync(p => p.PersonnelCode == vm.PersonnelCode, ct))
        {
            ModelState.AddModelError(nameof(vm.PersonnelCode), "این کد پرسنلی قبلاً استفاده شده است");
        }

        if (!ModelState.IsValid)
        {
            await PopulateDropdownsAsync(vm, ct);
            return View(vm);
        }

        var entity = new Personnel
        {
            FullName = vm.FullName.Trim(),
            PersonnelCode = vm.PersonnelCode?.Trim(),
            NationalCode = vm.NationalCode?.Trim(),
            Mobile = vm.Mobile?.Trim(),
            Positions = vm.SelectedPositions.Any() ? string.Join(",", vm.SelectedPositions) : null,
            DepartmentId = vm.DepartmentId,
            IsActive = vm.IsActive,
            Notes = vm.Notes?.Trim()
        };

        _db.Personnel.Add(entity);
        await _db.SaveChangesAsync(ct);

        TempData["Success"] = $"پرسنل «{entity.FullName}» با موفقیت ثبت شد";
        return RedirectToAction(nameof(Index));
    }

    // ==========================================
    // ویرایش
    // ==========================================
    [HasPermission("Personnel", "Edit")]
    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var entity = await _db.Personnel.FirstOrDefaultAsync(p => p.Id == id, ct);
        if (entity == null) return NotFound();

        var positions = string.IsNullOrEmpty(entity.Positions)
            ? new List<string>()
            : entity.Positions.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();

        var vm = new PersonnelViewModel
        {
            Id = entity.Id,
            FullName = entity.FullName,
            PersonnelCode = entity.PersonnelCode,
            NationalCode = entity.NationalCode,
            Mobile = entity.Mobile,
            SelectedPositions = positions,
            DepartmentId = entity.DepartmentId,
            IsActive = entity.IsActive,
            Notes = entity.Notes
        };

        await PopulateDropdownsAsync(vm, ct);
        return View(vm);
    }

    [HasPermission("Personnel", "Edit")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(PersonnelViewModel vm, CancellationToken ct)
    {
        if (vm.Id == null) return BadRequest();

        var entity = await _db.Personnel.FirstOrDefaultAsync(p => p.Id == vm.Id.Value, ct);
        if (entity == null) return NotFound();

        // چک یکتایی کد پرسنلی (به جز خود پرسنل)
        if (!string.IsNullOrWhiteSpace(vm.PersonnelCode) &&
            await _db.Personnel.AnyAsync(p => p.PersonnelCode == vm.PersonnelCode && p.Id != entity.Id, ct))
        {
            ModelState.AddModelError(nameof(vm.PersonnelCode), "این کد پرسنلی قبلاً استفاده شده است");
        }

        if (!ModelState.IsValid)
        {
            await PopulateDropdownsAsync(vm, ct);
            return View(vm);
        }

        entity.FullName = vm.FullName.Trim();
        entity.PersonnelCode = vm.PersonnelCode?.Trim();
        entity.NationalCode = vm.NationalCode?.Trim();
        entity.Mobile = vm.Mobile?.Trim();
        entity.Positions = vm.SelectedPositions.Any() ? string.Join(",", vm.SelectedPositions) : null;
        entity.DepartmentId = vm.DepartmentId;
        entity.IsActive = vm.IsActive;
        entity.Notes = vm.Notes?.Trim();
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);

        TempData["Success"] = "پرسنل با موفقیت ویرایش شد";
        return RedirectToAction(nameof(Index));
    }

    // ==========================================
    // حذف (Soft)
    // ==========================================
    [HasPermission("Personnel", "Delete")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var entity = await _db.Personnel.FirstOrDefaultAsync(p => p.Id == id, ct);
        if (entity == null) return NotFound();

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);

        TempData["Success"] = "پرسنل با موفقیت حذف شد";
        return RedirectToAction(nameof(Index));
    }

    // ==========================================
    // Helper
    // ==========================================
    private async Task PopulateDropdownsAsync(PersonnelViewModel vm, CancellationToken ct)
    {
        // لیست سمت‌های ثابت (مشترک بین همه واحدها)
        var allPositions = new List<(string Code, string Display)>
        {
            // ملتینگ
            ("Supervisore", "سوپروایزر"),
            ("SheftHeader", "سرشیفت"),
            ("GelMaker", "ملتر"),
            ("IpQc", "ناظر کنترل کیفی"),
            ("TankWasher", "شوینده تانک"),
            
            // تولید
            ("ProductionSupervisor", "سوپروایزر تولید"),
            ("ProductionOperator", "اپراتور تولید"),
            ("ProductionHelper", "کمک تولید"),
            
            // سورت
            ("SortOperator", "اپراتور سورت"),
            ("SortQC", "کنترل کیفی سورت"),
            
            // انبار
            ("WarehouseKeeper", "انباردار"),
            ("WarehouseHelper", "کمک انباردار"),
            
            // عمومی
            ("Manager", "مدیر"),
            ("Supervisor", "سرپرست"),
            ("Employee", "کارمند")
        };

        vm.AvailablePositions = allPositions.Select(p => new PositionItem
        {
            Code = p.Code,
            DisplayName = p.Display,
            IsSelected = vm.SelectedPositions.Contains(p.Code)
        }).ToList();

        vm.AvailableDepartments = await _db.Departments
            .Where(d => d.IsActive)
            .OrderBy(d => d.Name)
            .Select(d => new DepartmentOption { Id = d.Id, Name = d.Name })
            .ToListAsync(ct);
    }
}