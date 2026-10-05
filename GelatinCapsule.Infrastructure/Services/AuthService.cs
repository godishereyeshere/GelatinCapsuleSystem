using GelatinCapsule.Application.Common.Interfaces;
using GelatinCapsule.Application.Features.Auth.DTOs;
using GelatinCapsule.Domain.Entities.Security;
using GelatinCapsule.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GelatinCapsule.Infrastructure.Services;

public class AuthService : IAuthService
{
    private const int MaxFailedAttempts = 5;
    private const int LockoutMinutes = 15;

    private readonly ApplicationDbContext _db;
    private readonly PasswordHasher _passwordHasher;

    public AuthService(ApplicationDbContext db, PasswordHasher passwordHasher)
    {
        _db = db;
        _passwordHasher = passwordHasher;
    }

    public async Task<LoginResult> LoginAsync(string username, string password, string? ipAddress, CancellationToken ct = default)
    {
        var user = await _db.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Username == username && !u.IsDeleted, ct);

        if (user == null)
            return LoginResult.Fail("نام کاربری یا رمز عبور اشتباه است");

        if (!user.IsActive)
            return LoginResult.Fail("حساب کاربری شما غیرفعال است");

        if (user.IsLockedOut && user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow)
        {
            var remaining = (int)Math.Ceiling((user.LockoutEnd.Value - DateTime.UtcNow).TotalMinutes);
            return LoginResult.Fail($"حساب شما به دلیل تلاش‌های ناموفق قفل شده است. {remaining} دقیقه دیگر دوباره تلاش کنید");
        }

        if (!_passwordHasher.Verify(user.PasswordHash, password))
        {
            user.AccessFailedCount++;
            if (user.AccessFailedCount >= MaxFailedAttempts)
            {
                user.IsLockedOut = true;
                user.LockoutEnd = DateTime.UtcNow.AddMinutes(LockoutMinutes);
            }

            _db.AuditLogs.Add(new AuditLog
            {
                UserId = user.Id,
                Username = user.Username,
                Action = "LoginFailed",
                IpAddress = ipAddress,
                IsSuccess = false,
                ErrorMessage = "رمز عبور اشتباه"
            });

            await _db.SaveChangesAsync(ct);

            if (user.IsLockedOut)
                return LoginResult.Fail($"حساب شما به دلیل {MaxFailedAttempts} تلاش ناموفق، {LockoutMinutes} دقیقه قفل شد");

            return LoginResult.Fail("نام کاربری یا رمز عبور اشتباه است");
        }

        user.AccessFailedCount = 0;
        user.IsLockedOut = false;
        user.LockoutEnd = null;
        user.LastLoginAt = DateTime.UtcNow;
        user.LastLoginIp = ipAddress;

        _db.AuditLogs.Add(new AuditLog
        {
            UserId = user.Id,
            Username = user.Username,
            Action = "LoginSuccess",
            IpAddress = ipAddress,
            IsSuccess = true
        });

        await _db.SaveChangesAsync(ct);

        return LoginResult.Ok(user.Id, user.Username, user.FullName, user.MustChangePassword);
    }

    public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword, CancellationToken ct = default)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);
        if (user == null) return false;

        if (!_passwordHasher.Verify(user.PasswordHash, currentPassword))
            return false;

        user.PasswordHash = _passwordHasher.Hash(newPassword);
        user.MustChangePassword = false;
        user.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return true;
    }

    // 👇 این متد جدید اضافه شده
    public async Task<List<int>> GetUserRoleIdsAsync(int userId, CancellationToken ct = default)
    {
        return await _db.UserRoles
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.RoleId)
            .ToListAsync(ct);
    }
}