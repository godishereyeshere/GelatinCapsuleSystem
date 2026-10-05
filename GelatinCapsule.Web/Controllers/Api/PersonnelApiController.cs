using GelatinCapsule.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GelatinCapsule.Web.Controllers.Api;

[Authorize]
[Route("api/personnel")]
[ApiController]
public class PersonnelApiController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public PersonnelApiController(ApplicationDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// جستجوی پرسنل برای Select2
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string? q,
        [FromQuery] string? position,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 30,
        CancellationToken ct = default)
    {
        var query = _db.Personnel
            .Where(p => p.IsActive);

        // فیلتر بر اساس نام یا کد پرسنلی
        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(p =>
                p.FullName.Contains(q) ||
                (p.PersonnelCode != null && p.PersonnelCode.Contains(q)));
        }

        // فیلتر بر اساس سمت
        if (!string.IsNullOrWhiteSpace(position))
        {
            query = query.Where(p => p.Positions != null && p.Positions.Contains(position));
        }

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderBy(p => p.FullName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new
            {
                id = p.Id,
                text = p.FullName + (p.PersonnelCode != null ? " - " + p.PersonnelCode : ""),
                fullName = p.FullName,
                personnelCode = p.PersonnelCode,
                positions = p.Positions
            })
            .ToListAsync(ct);

        return Ok(new
        {
            results = items,
            pagination = new { more = (page * pageSize) < total }
        });
    }
}