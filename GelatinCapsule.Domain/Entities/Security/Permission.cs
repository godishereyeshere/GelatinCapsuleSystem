using GelatinCapsule.Domain.Common;

namespace GelatinCapsule.Domain.Entities.Security;

public class Permission : BaseEntity
{
    public string Code { get; set; } = null!;         // View, Create, Edit, Delete
    public string DisplayName { get; set; } = null!;  // مشاهده، ایجاد، ...
    public int SortOrder { get; set; } = 0;
}