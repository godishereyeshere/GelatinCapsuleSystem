using GelatinCapsule.Domain.Common;

namespace GelatinCapsule.Domain.Entities.Melting;

/// <summary>
/// انواع مورد تولید (ملت، پیست، DPI، HPMC، Enteric، ...)
/// </summary>
public class ProductionType : BaseEntity
{
    public string Code { get; set; } = null!;           // melt, pst, meltDPI, ...
    public string DisplayName { get; set; } = null!;    // ملت، پیست، ملت DPI، ...
    public string PType { get; set; } = null!;          // Gelatin, DPI, HPMC, ...
    public string CCode { get; set; } = null!;          // Y000, W000, ...
    public int PartCode { get; set; }                   // 3, 4, 1, 2, ...
    public int PermissionCode { get; set; }             // 8500, 85001, ...
    public decimal DefaultVol0 { get; set; }            // حجم اولیه پیش‌فرض
    public int SortOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }
    // Navigation
    public ICollection<ProductFormula> Formulas { get; set; } = new List<ProductFormula>();
}