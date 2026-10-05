using GelatinCapsule.Domain.Common;

namespace GelatinCapsule.Domain.Entities.Melting;

public class ProductFormula : BaseEntity
{
    public int ProductionTypeId { get; set; }
    public ProductionType ProductionType { get; set; } = null!;

    public int MaterialId { get; set; }
    public Material Material { get; set; } = null!;

    public decimal Percentage { get; set; }

    public int SortOrder { get; set; } = 0;
}