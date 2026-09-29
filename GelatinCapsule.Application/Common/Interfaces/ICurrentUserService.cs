namespace GelatinCapsule.Application.Common.Interfaces;

public interface ICurrentUserService
{
    int? UserId { get; }
    string? Username { get; }
    string? FullName { get; }
    bool IsAuthenticated { get; }
    IReadOnlyList<int> RoleIds { get; }
}