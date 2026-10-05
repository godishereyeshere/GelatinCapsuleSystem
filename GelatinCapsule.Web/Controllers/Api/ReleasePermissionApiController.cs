using GelatinCapsule.Application.Common.Interfaces;
using GelatinCapsule.Application.Features.Melting.Services;
using GelatinCapsule.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GelatinCapsule.Web.Controllers.Api;

[Authorize]
[Route("api/release-permission")]
[ApiController]
public class ReleasePermissionApiController : ControllerBase
{
    private readonly IReleasePermissionService _service;
    private readonly ApplicationDbContext _db;

    public ReleasePermissionApiController(
        IReleasePermissionService service,
        ApplicationDbContext db)
    {
        _service = service;
        _db = db;
    }

    /// <summary>
    /// محاسبه‌ی فرمول‌ها + گرفتن فرمول مواد + serial بعدی
    /// </summary>
    [HttpPost("calculate")]
    public async Task<IActionResult> Calculate(
        [FromBody] CalculateRequest request,
        CancellationToken ct)
    {
        // 0) پیدا کردن ProductionTypeId بر اساس CCode
        var productionTypeId = await _db.ProductionTypes
            .Where(x => x.CCode == request.CCode && x.IsActive)
            .Select(x => (int?)x.Id)
            .FirstOrDefaultAsync(ct);

        if (productionTypeId == null)
        {
            return BadRequest(new
            {
                error = $"نوع تولیدی با کد محصول «{request.CCode}» تعریف نشده است"
            });
        }

        // 1) محاسبات
        var result = ReleasePermissionCalculator.Calculate(
            request.Part, request.Vol0, request.Vis0, request.Vis1, request.LagTime);

        // 2) سریال بعدی
        var serial = await _service.GetNextSerialAsync(request.CCode, ct);

        // 3) فرمول مواد
        var formula = await _service.GetFormulaAsync(productionTypeId.Value, ct);

        // 4) محاسبه‌ی مقدار گرم هر ماده
        var materials = formula.Select(f => new
        {
            f.MaterialCode,
            f.MaterialDisplayName,
            f.Percentage,
            RequiredGr = Math.Round((decimal)f.Percentage / 100 * result.EqcCapsule, 4),
            f.SortOrder
        }).ToList();

        // 5) FeedRecNo + BatchNo
        var year = request.Farsidate.Replace("/", "").Substring(0, 4);
        var machineStr = request.Machine.ToString("D2");
        var partCode = request.Part == "melt" ? "3" : "4";
        var serialStr = serial.ToString("D4");
        var feedRecNo = $"{year}{machineStr}{partCode}{serialStr}";
        var batchPrefix = request.Part == "melt" ? "melt" : "pst";
        var batchNo = $"{batchPrefix}{serial}";

        return Ok(new
        {
            c0 = result.C0,
            c1 = result.C1,
            vol1 = result.Vol1,
            wei0 = result.Wei0,
            eqcCapsule = result.EqcCapsule,
            wtrNeed = result.WtrNeed,
            serial,
            feedRecNo,
            batchNo,
            materials
        });
    }

    /// <summary>
    /// گرفتن فرمول مواد برای یک CCode
    /// </summary>
    [HttpGet("formula/{cCode}")]
    public async Task<IActionResult> GetFormula(string cCode, CancellationToken ct)
    {
        var productionTypeId = await _db.ProductionTypes
            .Where(x => x.CCode == cCode && x.IsActive)
            .Select(x => (int?)x.Id)
            .FirstOrDefaultAsync(ct);

        if (productionTypeId == null)
        {
            return NotFound(new
            {
                error = $"نوع تولیدی با کد محصول «{cCode}» تعریف نشده است"
            });
        }

        var formula = await _service.GetFormulaAsync(productionTypeId.Value, ct);
        return Ok(formula);
    }
}

public class CalculateRequest
{
    public string Part { get; set; } = "melt";
    public string CCode { get; set; } = "Y000";
    public string Farsidate { get; set; } = "1405/07/08";
    public int Machine { get; set; } = 0;
    public decimal Vol0 { get; set; }
    public decimal Vis0 { get; set; }
    public decimal Vis1 { get; set; }
    public int LagTime { get; set; }
}