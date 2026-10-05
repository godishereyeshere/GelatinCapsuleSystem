namespace GelatinCapsule.Domain.Entities.Security;

public class RoleModuleAccess
{
    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;

    public int ModuleId { get; set; }
    public Module Module { get; set; } = null!;
}