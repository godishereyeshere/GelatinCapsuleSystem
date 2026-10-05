using GelatinCapsule.Domain.Common;

namespace GelatinCapsule.Domain.Entities.Security;

public class Department : BaseEntity
{
    public string Name { get; set; } = null!;

    // ساختار درختی (والد/فرزند)
    public int? ParentId { get; set; }
    public Department? Parent { get; set; }
    public ICollection<Department> Children { get; set; } = new List<Department>();

    // مدیر واحد
    public int? ManagerUserId { get; set; }
    public User? Manager { get; set; }

    // کاربران این واحد
    public ICollection<User> Users { get; set; } = new List<User>();

    public bool IsActive { get; set; } = true;
}