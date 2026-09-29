using System.Security.Claims;
using GelatinCapsule.Application.Common.Interfaces;

namespace GelatinCapsule.Web.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private List<int>? _roleIds;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public int? UserId
    {
        get
        {
            var id = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(id, out var userId) ? userId : null;
        }
    }

    public string? Username => User?.FindFirstValue(ClaimTypes.Name);
    public string? FullName => User?.FindFirstValue("FullName");
    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    public IReadOnlyList<int> RoleIds
    {
        get
        {
            if (_roleIds != null) return _roleIds;

            var roleClaims = User?.FindAll("RoleId") ?? Enumerable.Empty<Claim>();
            _roleIds = roleClaims
                .Select(c => int.TryParse(c.Value, out var id) ? id : 0)
                .Where(id => id > 0)
                .Distinct()
                .ToList();

            return _roleIds;
        }
    }
}