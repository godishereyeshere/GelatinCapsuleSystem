using GelatinCapsule.Domain.Entities.Security;
using Microsoft.EntityFrameworkCore;
using GelatinCapsule.Domain.Entities.Melting;
using GelatinCapsule.Domain.Entities.Hr;

namespace GelatinCapsule.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // ========== Security Tables ==========
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Module> Modules => Set<Module>();
    public DbSet<Form> Forms => Set<Form>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RoleFormPermission> RoleFormPermissions => Set<RoleFormPermission>();
    public DbSet<UserFormPermission> UserFormPermissions => Set<UserFormPermission>();
    public DbSet<UserModuleAccess> UserModuleAccesses => Set<UserModuleAccess>();
    public DbSet<RoleModuleAccess> RoleModuleAccesses => Set<RoleModuleAccess>();
    public DbSet<UserFormDataScope> UserFormDataScopes => Set<UserFormDataScope>();
    public DbSet<RoleFormDataScope> RoleFormDataScopes => Set<RoleFormDataScope>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    // ========== Melting Tables ==========
    public DbSet<ShiftInfo> MeltingShiftInfos => Set<ShiftInfo>();
    // ========== Melting Master Data ==========
    public DbSet<Material> Materials => Set<Material>();
    public DbSet<ProductFormula> ProductFormulas => Set<ProductFormula>();

    // ========== Melting Release Permission ==========
    public DbSet<ReleasePermission> ReleasePermissions => Set<ReleasePermission>();
    public DbSet<ReleasePermissionMaterial> ReleasePermissionMaterials => Set<ReleasePermissionMaterial>();

    public DbSet<ProductionType> ProductionTypes => Set<ProductionType>();

    public DbSet<ReleasePermissionGelatin> ReleasePermissionGelatins => Set<ReleasePermissionGelatin>();
    // ========== HR Tables ==========
    public DbSet<Personnel> Personnel => Set<Personnel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // این خط همه‌ی IEntityTypeConfiguration های توی این Assembly رو خودکار اعمال می‌کنه
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}