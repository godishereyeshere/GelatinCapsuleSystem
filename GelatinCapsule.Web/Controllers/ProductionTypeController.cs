using GelatinCapsule.Domain.Entities.Melting;
using GelatinCapsule.Infrastructure.Persistence;
using GelatinCapsule.Web.Authorization;
using GelatinCapsule.Web.Models.Melting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GelatinCapsule.Web.Controllers;

[HasPermission("ProductionType", "View")]
public class ProductionTypeController : Controller
{
    private readonly ApplicationDbContext _db;

    public ProductionTypeController(ApplicationDbContext db) => _db = db;

    // ==========================================
    // لیست
    // ==========================================
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var items = await _db.ProductionTypes
            .OrderBy(x => x.SortOrder)
            .ToListAsync(ct);

        return View(items);
    }

    // ==========================================
    // جزئیات
    // ==========================================
    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var item = await _db.ProductionTypes.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (item == null) return NotFound();
        return View(item);
    }

    // ==========================================
    // ساخت جدید
    // ==========================================
    [HasPermission("ProductionType", "Create")]
    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        var vm = new ProductionTypeViewModel();
        await PopulateDropdownsAsync(vm, ct);
        return View(vm);
    }

    [HasPermission("ProductionType", "Create")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductionTypeViewModel vm, CancellationToken ct)
    {
        if (await _db.ProductionTypes.AnyAsync(x => x.Code == vm.Code, ct))
            ModelState.AddModelError(nameof(vm.Code), "این کد قبلاً استفاده شده است");

        if (!ModelState.IsValid)
        {
            await PopulateDropdownsAsync(vm, ct);
            return View(vm);
        }

        var entity = new ProductionType
        {
            Code = vm.Code.Trim(),
            DisplayName = vm.DisplayName.Trim(),
            PType = vm.PType.Trim(),
            CCode = vm.CCode.Trim(),
            PartCode = vm.PartCode,
            PermissionCode = vm.PermissionCode,
            DefaultVol0 = vm.DefaultVol0,
            SortOrder = vm.SortOrder,
            IsActive = vm.IsActive,
            Notes = vm.Notes?.Trim()
        };

        _db.ProductionTypes.Add(entity);
        await _db.SaveChangesAsync(ct);

        TempData["Success"] = $"مورد تولید «{entity.DisplayName}» با موفقیت ثبت شد";
        return RedirectToAction(nameof(Index));
    }

    // ==========================================
    // ویرایش
    // ==========================================
    [HasPermission("ProductionType", "Edit")]
    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var entity = await _db.ProductionTypes.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity == null) return NotFound();

        var vm = new ProductionTypeViewModel
        {
            Id = entity.Id,
            Code = entity.Code,
            DisplayName = entity.DisplayName,
            PType = entity.PType,
            CCode = entity.CCode,
            PartCode = entity.PartCode,
            PermissionCode = entity.PermissionCode,
            DefaultVol0 = entity.DefaultVol0,
            SortOrder = entity.SortOrder,
            IsActive = entity.IsActive,
            Notes = entity.Notes
        };

        await PopulateDropdownsAsync(vm, ct);
        return View(vm);
    }

    [HasPermission("ProductionType", "Edit")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ProductionTypeViewModel vm, CancellationToken ct)
    {
        if (vm.Id == null) return BadRequest();

        var entity = await _db.ProductionTypes.FirstOrDefaultAsync(x => x.Id == vm.Id.Value, ct);
        if (entity == null) return NotFound();

        if (await _db.ProductionTypes.AnyAsync(x => x.Code == vm.Code && x.Id != entity.Id, ct))
            ModelState.AddModelError(nameof(vm.Code), "این کد قبلاً استفاده شده است");

        if (!ModelState.IsValid)
        {
            await PopulateDropdownsAsync(vm, ct);
            return View(vm);
        }

        entity.Code = vm.Code.Trim();
        entity.DisplayName = vm.DisplayName.Trim();
        entity.PType = vm.PType.Trim();
        entity.CCode = vm.CCode.Trim();
        entity.PartCode = vm.PartCode;
        entity.PermissionCode = vm.PermissionCode;
        entity.DefaultVol0 = vm.DefaultVol0;
        entity.SortOrder = vm.SortOrder;
        entity.IsActive = vm.IsActive;
        entity.Notes = vm.Notes?.Trim();
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);

        TempData["Success"] = "مورد تولید با موفقیت ویرایش شد";
        return RedirectToAction(nameof(Index));
    }

    // ==========================================
    // حذف نرم
    // ==========================================
    [HasPermission("ProductionType", "Delete")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var entity = await _db.ProductionTypes.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity == null) return NotFound();

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);

        TempData["Success"] = $"مورد تولید «{entity.DisplayName}» حذف شد";
        return RedirectToAction(nameof(Index));
    }
    // ==========================================
    // مدیریت فرمولاسیون
    // ==========================================
    // ==========================================
    // مدیریت فرمولاسیون
    // ==========================================
    [HasPermission("ProductionType", "Edit")]
    [HttpGet]
    public async Task<IActionResult> Formulas(int id, CancellationToken ct)
    {
        var pt = await _db.ProductionTypes.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (pt == null) return NotFound();

        var formulas = await _db.ProductFormulas
            .Where(f => f.ProductionTypeId == id)
            .Include(f => f.Material)
            .OrderBy(f => f.SortOrder)
            .ToListAsync(ct);

        var materials = await _db.Materials
            .Where(m => m.IsActive)
            .OrderBy(m => m.DisplayName)
            .ToListAsync(ct);

        var vm = new FormulaEditViewModel
        {
            ProductionTypeId = pt.Id,
            ProductionTypeCode = pt.Code,
            ProductionTypeDisplayName = pt.DisplayName,
            Rows = formulas.Select(f => new FormulaRowEditDto
            {
                MaterialId = f.MaterialId,
                MaterialCode = f.Material.Code,
                MaterialDisplayName = f.Material.DisplayName,
                Percentage = f.Percentage,
                SortOrder = f.SortOrder
            }).ToList(),
            AvailableMaterials = materials.Select(m => new MaterialOptionDto
            {
                Id = m.Id,
                Code = m.Code,
                DisplayName = m.DisplayName
            }).ToList()
        };

        return View(vm);
    }

    [HasPermission("ProductionType", "Edit")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Formulas(FormulaEditViewModel vm, CancellationToken ct)
    {
        var pt = await _db.ProductionTypes.FirstOrDefaultAsync(x => x.Id == vm.ProductionTypeId, ct);
        if (pt == null) return NotFound();

        // 1) پاک کردن فرمول‌های قبلی
        var existing = await _db.ProductFormulas
            .Where(f => f.ProductionTypeId == vm.ProductionTypeId)
            .ToListAsync(ct);
        _db.ProductFormulas.RemoveRange(existing);

        // 2) اضافه کردن فرمول‌های جدید
        var form = Request.Form;
        var materialIds = form["MaterialId"].ToArray();
        var percentages = form["Percentage"].ToArray();
        var sortOrders = form["SortOrder"].ToArray();

        for (int i = 0; i < materialIds.Length; i++)
        {
            if (string.IsNullOrEmpty(materialIds[i])) continue;
            if (!int.TryParse(materialIds[i], out var matId)) continue;

            var pctStr = percentages[i].Replace("٫", ".").Replace(",", ".");
            if (!decimal.TryParse(pctStr,
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out var pct)) continue;

            if (!int.TryParse(sortOrders[i], out var sort)) sort = i + 1;

            _db.ProductFormulas.Add(new ProductFormula
            {
                ProductionTypeId = vm.ProductionTypeId,
                MaterialId = matId,
                Percentage = pct,
                SortOrder = sort
            });
        }

        await _db.SaveChangesAsync(ct);

        TempData["Success"] = $"فرمولاسیون «{pt.DisplayName}» با موفقیت ذخیره شد";
        return RedirectToAction(nameof(Formulas), new { id = vm.ProductionTypeId });
    }

    // ==========================================
    // Helper
    // ==========================================
    private async Task PopulateDropdownsAsync(ProductionTypeViewModel vm, CancellationToken ct)
    {
        // CCode ها = لیست یکتای CCode از ReleasePermissions (یا هر جای دیگه)
        var cCodes = await _db.ProductionTypes
            .Select(pt => pt.CCode)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync(ct);

        // اگه ProductionTypes خالی بود، پیش‌فرض
        if (!cCodes.Any())
        {
            cCodes = new List<string> { "Y000", "W000" };
        }

        vm.AvailableCCodes = cCodes.Select(c => new CCodeOptionDto
        {
            Value = c,
            Label = c
        }).ToList();
    }
}