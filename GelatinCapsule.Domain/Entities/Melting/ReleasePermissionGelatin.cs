namespace GelatinCapsule.Domain.Entities.Melting;

/// <summary>
/// ژلاتین مصرفی به تفکیک کیفی و بچ (فقط برای Y000)
/// </summary>
public class ReleasePermissionGelatin
{
    public long Id { get; set; }
    public int ReleasePermissionId { get; set; }
    public ReleasePermission ReleasePermission { get; set; } = null!;

    public string GelatinName { get; set; } = null!;
    public decimal Amount { get; set; }
    public string? BatchNo { get; set; }
    public int SortOrder { get; set; } = 0;
}