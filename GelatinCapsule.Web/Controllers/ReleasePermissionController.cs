using GelatinCapsule.Application.Common.Helpers;
using GelatinCapsule.Application.Common.Interfaces;
using GelatinCapsule.Application.Features.DataScopes.Extensions;
using GelatinCapsule.Application.Features.Melting.Services;
using GelatinCapsule.Domain.Entities.Melting;
using GelatinCapsule.Infrastructure.Persistence;
using GelatinCapsule.Web.Authorization;
using GelatinCapsule.Web.Models.Melting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GelatinCapsule.Web.Controllers;

[HasPermission("ReleasePermission", "View")]
public class ReleasePermissionController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IDataScopeService _dataScope;
    private readonly ICurrentUserService _currentUser;
    private readonly IReleasePermissionService _service;

    public ReleasePermissionController(
        ApplicationDbContext db,
        IDataScopeService dataScope,
        ICurrentUserService currentUser,
        IReleasePermissionService service)
    {
        _db = db;
        _dataScope = dataScope;
        _currentUser = currentUser;
        _service = service;
    }

    // ==========================================
    // لیست
    // ==========================================
    public async Task<IActionResult> Index(string? search, CancellationToken ct)
    {
        var scope = await _dataScope.GetCurrentScopeAsync(ct);

        var query = _db.ReleasePermissions
            .ApplyDataScope(scope)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                x.FeedRecNo.Contains(search) ||
                x.BatchNo.Contains(search) ||
                x.Farsidate.Contains(search));
        }

        var items = await query
            .OrderByDescending(x => x.Id)
            .Take(200)
            .ToListAsync(ct);

        ViewBag.Search = search;
        return View(items);
    }

    // ==========================================
    // جزئیات
    // ==========================================
    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var item = await _db.ReleasePermissions
            .Include(x => x.Materials)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (item == null) return NotFound();
        return View(item);
    }

    // ==========================================
    // ساخت جدید - GET
    // ==========================================
    [HasPermission("ReleasePermission", "Create")]
    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        var vm = new ReleasePermissionViewModel
        {
            Farsidate = PersianDateHelper.ToPersianDateString(DateTime.Now),
            Shift = "صبح",
            CCode = "Y000",
            Vol0 = 943.15M,
            Vis0 = 1000M,
            Vis1 = 1000M,
            LagTime = 0
        };

        await PopulateCalculationsAsync(vm, ct);
        return View(vm);
    }

    // ==========================================
    // ساخت جدید - POST
    // ==========================================
    [HasPermission("ReleasePermission", "Create")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ReleasePermissionViewModel vm, CancellationToken ct)
    {
        // 1) محاسبات مجدد
        await PopulateCalculationsAsync(vm, ct);

        // 2) چک یکتایی FeedRecNo
        if (!string.IsNullOrEmpty(vm.FeedRecNo) &&
            await _db.ReleasePermissions.AnyAsync(x => x.FeedRecNo == vm.FeedRecNo, ct))
        {
            ModelState.AddModelError(string.Empty,
                $"رکورد تانک {vm.FeedRecNo} قبلاً ثبت شده است");
        }

        if (!ModelState.IsValid)
            return View(vm);

        // 3) ساخت Entity
        var part = vm.CCode == "Y000" ? "melt" : "pst";
        const string pType = "Gelatin";
        const int permission = 8500;

        var entity = new ReleasePermission
        {
            FeedRecNo = vm.FeedRecNo!,
            BatchNo = vm.BatchNo!,
            Serial = vm.Serial ?? 0,

            Farsidate = vm.Farsidate,
            Shift = vm.Shift,
            FarsiYear = vm.Farsidate.Substring(0, 4),

            Part = part,
            PType = pType,
            CCode = vm.CCode,
            Machine = vm.Machine,
            Permission = permission,

            TankNo = vm.TankNo,

            Vol0 = vm.Vol0,
            Vis0 = vm.Vis0,
            Vis1 = vm.Vis1,
            LagTime = vm.LagTime,

            C0 = vm.C0 ?? 0,
            C1 = vm.C1 ?? 0,
            Vol1 = vm.Vol1 ?? 0,
            Wei0 = vm.Wei0 ?? 0,
            EqcCapsule = vm.EqcCapsule ?? 0,
            WtrNeed = vm.WtrNeed ?? 0,

            Company = vm.Company,
            ProductName = vm.ProductName,
            ColorCode = vm.ColorCode,
            LicenseSerial = vm.LicenseSerial,
            RelatedMeltId = vm.RelatedMeltId,
            Notes = vm.Notes,

            CreatedBy = _currentUser.UserId,
            OwnerDepartmentId = null
        };

        _db.ReleasePermissions.Add(entity);
        await _db.SaveChangesAsync(ct);

        // 4) ذخیره‌ی مواد
        // TODO: بعد از آپدیت ReleasePermissionViewModel، این خط رو درست می‌کنیم
        var materials = new List<ProductFormulaDto>();
        foreach (var m in materials)
        {
            _db.ReleasePermissionMaterials.Add(new ReleasePermissionMaterial
            {
                ReleasePermissionId = entity.Id,
                MaterialCode = m.MaterialCode,
                RequiredGr = Math.Round(m.Percentage / 100 * entity.EqcCapsule, 4),
                SortOrder = m.SortOrder
            });
        }
        await _db.SaveChangesAsync(ct);

        TempData["Success"] = $"مجوز ساخت {entity.BatchNo} با موفقیت ثبت شد";
        return RedirectToAction(nameof(Details), new { id = entity.Id });
    }

    // ==========================================
    // ویرایش - GET
    // ==========================================
    [HasPermission("ReleasePermission", "Edit")]
    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var scope = await _dataScope.GetCurrentScopeAsync(ct);

        var entity = await _db.ReleasePermissions
            .ApplyDataScope(scope)
            .Include(x => x.Gelatins)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (entity == null) return NotFound();

        var vm = new ReleasePermissionViewModel
        {
            Id = entity.Id,
            Farsidate = entity.Farsidate,
            Shift = entity.Shift,
            CCode = entity.CCode,
            Machine = entity.Machine,
            Vol0 = entity.Vol0,
            Vis0 = entity.Vis0,
            Vis1 = entity.Vis1,
            LagTime = entity.LagTime,
            TankNo = entity.TankNo,
            Notes = entity.Notes,
            Company = entity.Company,
            ProductName = entity.ProductName,
            ColorCode = entity.ColorCode,
            LicenseSerial = entity.LicenseSerial,
            RelatedMeltId = entity.RelatedMeltId
        };

        await PopulateCalculationsAsync(vm, ct);
        return View(vm);
    }

    // ==========================================
    // ویرایش - POST
    // ==========================================
    [HasPermission("ReleasePermission", "Edit")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ReleasePermissionViewModel vm, CancellationToken ct)
    {
        if (vm.Id == null) return BadRequest();

        var scope = await _dataScope.GetCurrentScopeAsync(ct);

        var entity = await _db.ReleasePermissions
            .ApplyDataScope(scope)
            .Include(x => x.Materials)
            .Include(x => x.Gelatins)
            .FirstOrDefaultAsync(x => x.Id == vm.Id.Value, ct);

        if (entity == null) return NotFound();

        // 1) محاسبات مجدد
        var part = vm.CCode == "Y000" ? "melt" : "pst";
        var calc = ReleasePermissionCalculator.Calculate(
            part, vm.Vol0, vm.Vis0, vm.Vis1, vm.LagTime);

        // 2) آپدیت فیلدها
        entity.Farsidate = vm.Farsidate;
        entity.Shift = vm.Shift;
        entity.CCode = vm.CCode;
        entity.Part = part;
        entity.Machine = vm.Machine;
        entity.TankNo = vm.TankNo;
        entity.Notes = vm.Notes;
        entity.Company = vm.Company;
        entity.ProductName = vm.ProductName;
        entity.ColorCode = vm.ColorCode;
        entity.LicenseSerial = vm.LicenseSerial;
        entity.RelatedMeltId = vm.RelatedMeltId;

        entity.Vol0 = vm.Vol0;
        entity.Vis0 = vm.Vis0;
        entity.Vis1 = vm.Vis1;
        entity.LagTime = vm.LagTime;

        entity.C0 = calc.C0;
        entity.C1 = calc.C1;
        entity.Vol1 = calc.Vol1;
        entity.Wei0 = calc.Wei0;
        entity.EqcCapsule = calc.EqcCapsule;
        entity.WtrNeed = calc.WtrNeed;

        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = _currentUser.UserId;

        // 3) حذف مواد قبلی
        _db.ReleasePermissionMaterials.RemoveRange(entity.Materials);

        // 4) ذخیره‌ی مواد جدید
        // پیدا کردن ProductionTypeId
        var productionTypeId = await GetProductionTypeIdFromCCodeAsync(vm.CCode, ct);
        if (productionTypeId == null)
        {
            ModelState.AddModelError(string.Empty,
                $"نوع تولیدی با کد محصول «{vm.CCode}» تعریف نشده است.");
            return View(vm);
        }

        var materials = await _service.GetFormulaAsync(productionTypeId.Value, ct);

        foreach (var m in materials)
        {
            _db.ReleasePermissionMaterials.Add(new ReleasePermissionMaterial
            {
                ReleasePermissionId = entity.Id,
                MaterialCode = m.MaterialCode,
                RequiredGr = Math.Round(m.Percentage / 100 * entity.EqcCapsule, 4),
                SortOrder = m.SortOrder
            });
        }

        await _db.SaveChangesAsync(ct);

        TempData["Success"] = "مجوز با موفقیت ویرایش شد";
        return RedirectToAction(nameof(Details), new { id = entity.Id });
    }

    // ==========================================
    // حذف نرم
    // ==========================================
    [HasPermission("ReleasePermission", "Delete")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var scope = await _dataScope.GetCurrentScopeAsync(ct);

        var entity = await _db.ReleasePermissions
            .ApplyDataScope(scope)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (entity == null) return NotFound();

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = _currentUser.UserId;

        await _db.SaveChangesAsync(ct);

        TempData["Success"] = $"مجوز {entity.BatchNo} حذف شد";
        return RedirectToAction(nameof(Index));
    }

    // ==========================================
    // چاپ
    // ==========================================
    [HasPermission("ReleasePermission", "Print")]
    [HttpGet]
    public async Task<IActionResult> Print(int id, CancellationToken ct)
    {
        var entity = await _db.ReleasePermissions
            .Include(x => x.Materials)
            .Include(x => x.Gelatins)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (entity == null) return NotFound();

        return entity.CCode == "Y000"
            ? View("PrintMelt", entity)
            : View("PrintPst", entity);
    }

    // ==========================================
    // Helper — پر کردن محاسبات
    // ==========================================
    private async Task PopulateCalculationsAsync(ReleasePermissionViewModel vm, CancellationToken ct)
    {
        var part = vm.CCode == "Y000" ? "melt" : "pst";
        vm.ColorCode = vm.CCode == "Y000" ? "300" : "400";

        // 1) محاسبات
        var calc = ReleasePermissionCalculator.Calculate(
            part, vm.Vol0, vm.Vis0, vm.Vis1, vm.LagTime);

        vm.C0 = calc.C0;
        vm.C1 = calc.C1;
        vm.Vol1 = calc.Vol1;
        vm.Wei0 = calc.Wei0;
        vm.EqcCapsule = calc.EqcCapsule;
        vm.WtrNeed = calc.WtrNeed;

        // 2) Serial + FeedRecNo + BatchNo
        var serial = await _service.GetNextSerialAsync(vm.CCode, ct);
        vm.Serial = serial;

        var year = vm.Farsidate.Replace("/", "").Substring(0, 4);
        var machineStr = vm.Machine.ToString("D2");
        var partCode = part == "melt" ? "3" : "4";
        var serialStr = serial.ToString("D4");

        vm.FeedRecNo = $"{year}{machineStr}{partCode}{serialStr}";
        vm.BatchNo = $"{(part == "melt" ? "melt" : "pst")}{serial}";

        // 3) فرمول مواد
        // 3) فرمول مواد
        // 3) فرمول مواد — موقتاً خالی (بعداً از ProductionTypeId می‌خونیم)
        vm.FormulaRows = new List<FormulaRowViewModel>();
    }

    private async Task<int?> GetProductionTypeIdFromCCodeAsync(string cCode, CancellationToken ct)
    {
        return await _db.ProductionTypes
            .Where(x => x.CCode == cCode && x.IsActive)
            .Select(x => (int?)x.Id)
            .FirstOrDefaultAsync(ct);
    }
}