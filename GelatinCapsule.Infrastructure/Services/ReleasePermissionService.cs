using GelatinCapsule.Application.Common.Interfaces;
using GelatinCapsule.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GelatinCapsule.Infrastructure.Services;

public class ReleasePermissionService : IReleasePermissionService
{
    private readonly ApplicationDbContext _db;

    public ReleasePermissionService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<int> GetNextSerialAsync(string cCode, CancellationToken ct = default)
    {
        var maxSerial = await _db.ReleasePermissions
            .Where(x => x.CCode == cCode)
            .Select(x => (int?)x.Serial)
            .MaxAsync(ct);

        return (maxSerial ?? 0) + 1;
    }

    public async Task<List<ProductFormulaDto>> GetFormulaAsync(int productionTypeId, CancellationToken ct = default)
    {
        return await _db.ProductFormulas
            .Where(pf => pf.ProductionTypeId == productionTypeId)
            .OrderBy(pf => pf.SortOrder)
            .Select(pf => new ProductFormulaDto
            {
                MaterialCode = pf.Material.Code,
                MaterialDisplayName = pf.Material.DisplayName,
                Percentage = pf.Percentage,
                SortOrder = pf.SortOrder
            })
            .ToListAsync(ct);
    }
}