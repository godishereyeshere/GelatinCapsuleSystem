using GelatinCapsule.Application.Features.Melting.DTOs;

namespace GelatinCapsule.Application.Common.Interfaces;

public interface IReleasePermissionService
{
    Task<int> GetNextSerialAsync(string cCode, CancellationToken ct = default);
    Task<List<ProductFormulaDto>> GetFormulaAsync(int productionTypeId, CancellationToken ct = default);
}

public class ProductFormulaDto
{
    public string MaterialCode { get; set; } = null!;
    public string MaterialDisplayName { get; set; } = null!;
    public decimal Percentage { get; set; }
    public int SortOrder { get; set; }
}