using GelatinCapsule.Application.Features.Auth.DTOs;

namespace GelatinCapsule.Application.Common.Interfaces;

public interface IAuthService
{
    Task<LoginResult> LoginAsync(string username, string password, string? ipAddress, CancellationToken ct = default);
    Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword, CancellationToken ct = default);
    Task<List<int>> GetUserRoleIdsAsync(int userId, CancellationToken ct = default);
}