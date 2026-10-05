using GelatinCapsule.Domain.Enums;

namespace GelatinCapsule.Domain.Entities.Security;

public class UserFormDataScope
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int FormId { get; set; }
    public Form Form { get; set; } = null!;

    public DataScope Scope { get; set; } = DataScope.OnlyOwn;
}