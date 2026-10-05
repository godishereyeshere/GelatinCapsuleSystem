namespace GelatinCapsule.Domain.Entities.Melting;

/// <summary>
/// مواد مصرفی هر مجوز ساخت (Detail)
/// </summary>
public class ReleasePermissionMaterial
{
    public long Id { get; set; }
    public int ReleasePermissionId { get; set; }
    public ReleasePermission ReleasePermission { get; set; } = null!;

    public string MaterialCode { get; set; } = null!;   // Dgl, Aca, ...
    public decimal RequiredGr { get; set; }             // مقدار گرم
    public int SortOrder { get; set; } = 0;
}