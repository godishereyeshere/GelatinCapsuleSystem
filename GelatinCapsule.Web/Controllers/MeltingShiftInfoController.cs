using GelatinCapsule.Application.Common.Helpers;
using GelatinCapsule.Application.Common.Interfaces;
using GelatinCapsule.Application.Features.DataScopes.Extensions;
using GelatinCapsule.Domain.Entities.Melting;
using GelatinCapsule.Infrastructure.Persistence;
using GelatinCapsule.Web.Authorization;
using GelatinCapsule.Web.Models.Melting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GelatinCapsule.Web.Controllers;

[HasPermission("MeltingShiftInfo", "View")]
public class MeltingShiftInfoController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IDataScopeService _dataScope;
    private readonly ICurrentUserService _currentUser;

    public MeltingShiftInfoController(
        ApplicationDbContext db,
        IDataScopeService dataScope,
        ICurrentUserService currentUser)
    {
        _db = db;
        _dataScope = dataScope;
        _currentUser = currentUser;
    }

    // ==========================================
    // لیست
    // ==========================================
    public async Task<IActionResult> Index(string? farsidate, CancellationToken ct)
    {
        var scope = await _dataScope.GetCurrentScopeAsync(ct);

        var query = _db.MeltingShiftInfos
            .ApplyDataScope(scope)
            .AsQueryable();

        if (!string.IsNullOrEmpty(farsidate))
            query = query.Where(x => x.Farsidate.Contains(farsidate));

        var items = await query
            .OrderByDescending(x => x.Farsidate)
            .ThenBy(x => x.Shift)
            .ToListAsync(ct);

        ViewBag.FilterDate = farsidate;
        return View(items);
    }

    // ==========================================
    // ساخت جدید
    // ==========================================
    [HasPermission("MeltingShiftInfo", "Create")]
    [HttpGet]
    public IActionResult Create()
    {
        var now = DateTime.Now;

        var vm = new ShiftInfoViewModel
        {
            Farsidate = PersianDateHelper.ToPersianDateString(now),
            Shift = "صبح"
        };

        return View(vm);
    }

    [HasPermission("MeltingShiftInfo", "Create")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ShiftInfoViewModel vm, CancellationToken ct)
    {
        // چک یکتایی (تاریخ + شیفت)
        var exists = await _db.MeltingShiftInfos
            .AnyAsync(x => x.Farsidate == vm.Farsidate && x.Shift == vm.Shift, ct);

        if (exists)
            ModelState.AddModelError(string.Empty,
                $"برای تاریخ {PersianDateHelper.FormatFarsiDate(vm.Farsidate)} شیفت «{vm.Shift}» قبلاً ثبت شده است");

        if (!ModelState.IsValid)
            return View(vm);

        var now = DateTime.Now;

        var entity = new ShiftInfo
        {
            Farsidate = vm.Farsidate.Trim(),
            Shift = vm.Shift.Trim(),
            Group = vm.Group?.Trim(),

            Supervisore = vm.Supervisore?.Trim(),
            SheftHeader = vm.SheftHeader?.Trim(),
            GelMaker1 = vm.GelMaker1?.Trim(),
            GelMaker2 = vm.GelMaker2?.Trim(),
            IpQc = vm.IpQc?.Trim(),
            TankWasher = vm.TankWasher?.Trim(),
            TankWasher2 = vm.TankWasher2?.Trim(),
            HelpMelter = vm.HelpMelter?.Trim(),

            SheftHeaderRep = vm.SheftHeaderRep?.Trim(),
            SheftHeaderRepFrom = vm.SheftHeaderRepFrom?.Trim(),
            GelMaker1Rep = vm.GelMaker1Rep?.Trim(),
            GelMaker1RepFrom = vm.GelMaker1RepFrom?.Trim(),
            GelMaker2Rep = vm.GelMaker2Rep?.Trim(),
            GelMaker2RepFrom = vm.GelMaker2RepFrom?.Trim(),
            TankWasherRep = vm.TankWasherRep?.Trim(),
            TankWasherRepFrom = vm.TankWasherRepFrom?.Trim(),

            Mildate = now,
            Momtime = now.ToString("HH:mm:ss"),
            CurrentDay = PersianDateHelper.GetDayOfWeekName(now),
            YearF = PersianDateHelper.GetYear(now),

            CreatedBy = _currentUser.UserId,
            OwnerDepartmentId = _currentUser.UserId.HasValue
                ? (await _db.Users.FindAsync(new object[] { _currentUser.UserId.Value }, ct))?.DepartmentId
                : null
        };

        _db.MeltingShiftInfos.Add(entity);
        await _db.SaveChangesAsync(ct);

        TempData["Success"] = "اطلاعات شیفت با موفقیت ثبت شد";
        return RedirectToAction(nameof(Index));
    }

    // ==========================================
    // ویرایش
    // ==========================================
    [HasPermission("MeltingShiftInfo", "Edit")]
    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var scope = await _dataScope.GetCurrentScopeAsync(ct);

        var entity = await _db.MeltingShiftInfos
            .ApplyDataScope(scope)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (entity == null) return NotFound();

        var vm = new ShiftInfoViewModel
        {
            Id = entity.Id,
            Farsidate = entity.Farsidate,
            Shift = entity.Shift,
            Group = entity.Group,
            Supervisore = entity.Supervisore,
            SheftHeader = entity.SheftHeader,
            GelMaker1 = entity.GelMaker1,
            GelMaker2 = entity.GelMaker2,
            IpQc = entity.IpQc,
            TankWasher = entity.TankWasher,
            TankWasher2 = entity.TankWasher2,
            HelpMelter = entity.HelpMelter,
            SheftHeaderRep = entity.SheftHeaderRep,
            SheftHeaderRepFrom = entity.SheftHeaderRepFrom,
            GelMaker1Rep = entity.GelMaker1Rep,
            GelMaker1RepFrom = entity.GelMaker1RepFrom,
            GelMaker2Rep = entity.GelMaker2Rep,
            GelMaker2RepFrom = entity.GelMaker2RepFrom,
            TankWasherRep = entity.TankWasherRep,
            TankWasherRepFrom = entity.TankWasherRepFrom
        };

        return View(vm);
    }

    [HasPermission("MeltingShiftInfo", "Edit")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ShiftInfoViewModel vm, CancellationToken ct)
    {
        if (vm.Id == null) return BadRequest();

        var scope = await _dataScope.GetCurrentScopeAsync(ct);

        var entity = await _db.MeltingShiftInfos
            .ApplyDataScope(scope)
            .FirstOrDefaultAsync(x => x.Id == vm.Id.Value, ct);

        if (entity == null) return NotFound();

        // چک یکتایی (به جز خود رکورد)
        var exists = await _db.MeltingShiftInfos
            .AnyAsync(x => x.Farsidate == vm.Farsidate
                        && x.Shift == vm.Shift
                        && x.Id != entity.Id, ct);

        if (exists)
            ModelState.AddModelError(string.Empty,
                $"برای تاریخ {PersianDateHelper.FormatFarsiDate(vm.Farsidate)} شیفت «{vm.Shift}» قبلاً ثبت شده است");

        if (!ModelState.IsValid)
            return View(vm);

        entity.Farsidate = vm.Farsidate.Trim();
        entity.Shift = vm.Shift.Trim();
        entity.Group = vm.Group?.Trim();

        entity.Supervisore = vm.Supervisore?.Trim();
        entity.SheftHeader = vm.SheftHeader?.Trim();
        entity.GelMaker1 = vm.GelMaker1?.Trim();
        entity.GelMaker2 = vm.GelMaker2?.Trim();
        entity.IpQc = vm.IpQc?.Trim();
        entity.TankWasher = vm.TankWasher?.Trim();
        entity.TankWasher2 = vm.TankWasher2?.Trim();
        entity.HelpMelter = vm.HelpMelter?.Trim();

        entity.SheftHeaderRep = vm.SheftHeaderRep?.Trim();
        entity.SheftHeaderRepFrom = vm.SheftHeaderRepFrom?.Trim();
        entity.GelMaker1Rep = vm.GelMaker1Rep?.Trim();
        entity.GelMaker1RepFrom = vm.GelMaker1RepFrom?.Trim();
        entity.GelMaker2Rep = vm.GelMaker2Rep?.Trim();
        entity.GelMaker2RepFrom = vm.GelMaker2RepFrom?.Trim();
        entity.TankWasherRep = vm.TankWasherRep?.Trim();
        entity.TankWasherRepFrom = vm.TankWasherRepFrom?.Trim();

        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = _currentUser.UserId;

        await _db.SaveChangesAsync(ct);

        TempData["Success"] = "اطلاعات شیفت با موفقیت ویرایش شد";
        return RedirectToAction(nameof(Index));
    }

    // ==========================================
    // حذف (Soft)
    // ==========================================
    [HasPermission("MeltingShiftInfo", "Delete")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var scope = await _dataScope.GetCurrentScopeAsync(ct);

        var entity = await _db.MeltingShiftInfos
            .ApplyDataScope(scope)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (entity == null) return NotFound();

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = _currentUser.UserId;

        await _db.SaveChangesAsync(ct);

        TempData["Success"] = "اطلاعات شیفت با موفقیت حذف شد";
        return RedirectToAction(nameof(Index));
    }
}