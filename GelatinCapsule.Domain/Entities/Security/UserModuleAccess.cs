namespace GelatinCapsule.Domain.Entities.Security;

public class UserModuleAccess
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int ModuleId { get; set; }
    public Module Module { get; set; } = null!;
}