using GelatinCapsule.Domain.Enums;

namespace GelatinCapsule.Domain.Entities.Security;

public class RoleFormDataScope
{
    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;

    public int FormId { get; set; }
    public Form Form { get; set; } = null!;

    public DataScope Scope { get; set; } = DataScope.OnlyOwn;
}