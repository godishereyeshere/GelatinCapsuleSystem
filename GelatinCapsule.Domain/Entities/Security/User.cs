using GelatinCapsule.Domain.Common;

namespace GelatinCapsule.Domain.Entities.Security;

public class User : BaseEntity
{
    // اطلاعات ورود
    public string Username { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string? SecurityStamp { get; set; }

    // اطلاعات هویتی
    public string FullName { get; set; } = null!;
    public string? PersonnelCode { get; set; }
    public string? NationalCode { get; set; }
    public string? Email { get; set; }
    public string? Mobile { get; set; }

    // سازمانی
    public int? DepartmentId { get; set; }
    public Department? Department { get; set; }

    // وضعیت حساب
    public bool IsActive { get; set; } = true;
    public bool IsLockedOut { get; set; } = false;
    public DateTime? LockoutEnd { get; set; }
    public int AccessFailedCount { get; set; } = 0;
    public bool MustChangePassword { get; set; } = false;
    public bool TwoFactorEnabled { get; set; } = false;

    // آخرین ورود
    public DateTime? LastLoginAt { get; set; }
    public string? LastLoginIp { get; set; }

    // Navigation Properties (روابط)
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<UserModuleAccess> ModuleAccesses { get; set; } = new List<UserModuleAccess>();
    public ICollection<UserFormPermission> FormPermissions { get; set; } = new List<UserFormPermission>();
    public ICollection<UserFormDataScope> FormDataScopes { get; set; } = new List<UserFormDataScope>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}