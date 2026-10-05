namespace GelatinCapsule.Web.Models.Melting;

public class FormulaEditViewModel
{
    public int ProductionTypeId { get; set; }
    public string ProductionTypeCode { get; set; } = null!;
    public string ProductionTypeDisplayName { get; set; } = null!;

    public List<FormulaRowEditDto> Rows { get; set; } = new();
    public List<MaterialOptionDto> AvailableMaterials { get; set; } = new();
}

public class FormulaRowEditDto
{
    public int MaterialId { get; set; }
    public string MaterialCode { get; set; } = null!;
    public string MaterialDisplayName { get; set; } = null!;
    public decimal Percentage { get; set; }
    public int SortOrder { get; set; }
}

public class MaterialOptionDto
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
}