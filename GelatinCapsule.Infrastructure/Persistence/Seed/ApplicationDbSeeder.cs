using GelatinCapsule.Domain.Entities.Security;
using GelatinCapsule.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace GelatinCapsule.Infrastructure.Persistence.Seed;

public static class ApplicationDbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext db)
    {
        // ============ 1) Permissions ============
        if (!await db.Permissions.AnyAsync())
        {
            db.Permissions.AddRange(
                new Permission { Code = "View", DisplayName = "مشاهده", SortOrder = 1 },
                new Permission { Code = "Create", DisplayName = "ایجاد", SortOrder = 2 },
                new Permission { Code = "Edit", DisplayName = "ویرایش", SortOrder = 3 },
                new Permission { Code = "Delete", DisplayName = "حذف", SortOrder = 4 },
                new Permission { Code = "Print", DisplayName = "چاپ", SortOrder = 5 },
                new Permission { Code = "Export", DisplayName = "خروجی اکسل", SortOrder = 6 },
                new Permission { Code = "Approve", DisplayName = "تایید", SortOrder = 7 }
            );
            await db.SaveChangesAsync();
        }

        // ============ 2) Modules ============
        if (!await db.Modules.AnyAsync())
        {
            db.Modules.AddRange(
                new Module { Code = "Production", Name = "Production", DisplayName = "تولید", SortOrder = 1 },
                new Module { Code = "Warehouse", Name = "Warehouse", DisplayName = "انبار", SortOrder = 2 },
                new Module { Code = "QC", Name = "QC", DisplayName = "کنترل کیفیت", SortOrder = 3 },
                new Module { Code = "Sales", Name = "Sales", DisplayName = "فروش", SortOrder = 4 },
                new Module { Code = "Melting", Name = "Melting", DisplayName = "ملتینگ (ذوب)", SortOrder = 5 },
                new Module { Code = "Admin", Name = "Admin", DisplayName = "مدیریت سیستم", SortOrder = 99 }
            );
            await db.SaveChangesAsync();
        }

        // ============ 3) Forms ============
        // 3) Forms
        if (!await db.Forms.AnyAsync())
        {
            var adminModule = await db.Modules.FirstAsync(m => m.Code == "Admin");
            var meltingModule = await db.Modules.FirstAsync(m => m.Code == "Melting");

            db.Forms.AddRange(
                // Admin Forms
                new Form { ModuleId = adminModule.Id, Code = "Users", DisplayName = "مدیریت کاربران", ControllerName = "Users", ShowInMenu = true, SortOrder = 1 },
                new Form { ModuleId = adminModule.Id, Code = "Roles", DisplayName = "مدیریت نقش‌ها", ControllerName = "Roles", ShowInMenu = true, SortOrder = 2 },
                new Form { ModuleId = adminModule.Id, Code = "Permissions", DisplayName = "مدیریت دسترسی‌ها", ControllerName = "Permissions", ShowInMenu = true, SortOrder = 3 },

                // Melting Forms
                new Form { ModuleId = meltingModule.Id, Code = "MeltingShiftInfo", DisplayName = "اطلاعات شیفت ملتینگ", ControllerName = "MeltingShiftInfo", ShowInMenu = true, SortOrder = 1 }
            );
            await db.SaveChangesAsync();
        }

        // ============ 4) Role Administrator ============
        var adminRole = await db.Roles.FirstOrDefaultAsync(r => r.Name == "Administrator");
        if (adminRole == null)
        {
            adminRole = new Role
            {
                Name = "Administrator",
                DisplayName = "مدیر سیستم",
                Description = "دسترسی کامل به همه بخش‌ها",
                IsSystemRole = true,
                IsActive = true
            };
            db.Roles.Add(adminRole);
            await db.SaveChangesAsync();
        }

        // ============ 5) Role Module Access ============
        if (!await db.RoleModuleAccesses.AnyAsync(rma => rma.RoleId == adminRole.Id))
        {
            var allModules = await db.Modules.ToListAsync();
            foreach (var module in allModules)
            {
                db.RoleModuleAccesses.Add(new RoleModuleAccess
                {
                    RoleId = adminRole.Id,
                    ModuleId = module.Id
                });
            }
            await db.SaveChangesAsync();
        }

        // ============ 6) Role Form Permissions ============
        if (!await db.RoleFormPermissions.AnyAsync(rfp => rfp.RoleId == adminRole.Id))
        {
            var allForms = await db.Forms.ToListAsync();
            var allPermissions = await db.Permissions.ToListAsync();

            foreach (var form in allForms)
            {
                foreach (var perm in allPermissions)
                {
                    db.RoleFormPermissions.Add(new RoleFormPermission
                    {
                        RoleId = adminRole.Id,
                        FormId = form.Id,
                        PermissionId = perm.Id
                    });
                }
            }
            await db.SaveChangesAsync();
        }

        // ============ 7) Admin User ============
        var adminUser = await db.Users.FirstOrDefaultAsync(u => u.Username == "admin");
        if (adminUser == null)
        {
            var hasher = new PasswordHasher();

            adminUser = new User
            {
                Username = "admin",
                PasswordHash = hasher.Hash("Admin@123"),
                FullName = "مدیر سیستم",
                IsActive = true,
                MustChangePassword = false
            };
            db.Users.Add(adminUser);
            await db.SaveChangesAsync();

            db.UserRoles.Add(new UserRole
            {
                UserId = adminUser.Id,
                RoleId = adminRole.Id
            });
            await db.SaveChangesAsync();
        }
    }
}