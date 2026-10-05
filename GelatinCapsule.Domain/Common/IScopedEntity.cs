namespace GelatinCapsule.Domain.Common;

/// <summary>
/// موجودیت‌هایی که از Row Level Security پشتیبانی می‌کنن.
/// هر entity که این interface رو پیاده کنه، خودکار فیلتر میشه.
/// </summary>
public interface IScopedEntity
{
    int? CreatedBy { get; set; }
    int? OwnerDepartmentId { get; set; }
}