using GelatinCapsule.Domain.Common;  // اگر لازم بود

namespace GelatinCapsule.Domain.Common;

public abstract class BaseEntity : IScopedEntity   // 👈 تغییر
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public int? OwnerDepartmentId { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; } = false;
}