namespace GelatinCapsule.Domain.Entities.Security;

public class UserFormPermission
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int FormId { get; set; }
    public Form Form { get; set; } = null!;

    public int PermissionId { get; set; }
    public Permission Permission { get; set; } = null!;

    // true = Grant (اجازه), false = Deny (منع صریح)
    public bool IsGranted { get; set; } = true;
}