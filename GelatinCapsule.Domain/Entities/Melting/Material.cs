using GelatinCapsule.Domain.Common;

namespace GelatinCapsule.Domain.Entities.Melting;

/// <summary>
/// مواد پایه‌ی مصرفی در ملتینگ (Dgl, Aca, Gso, ...)
/// </summary>
public class Material : BaseEntity
{
    public string Code { get; set; } = null!;         // Dgl, Aca, Gso
    public string DisplayName { get; set; } = null!;  // Dry Gelatin, Acetic Acid, ...
    public bool IsActive { get; set; } = true;
}