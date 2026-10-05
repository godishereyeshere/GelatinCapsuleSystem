namespace GelatinCapsule.Application.Features.DataScopes.DTOs;

public class DataScopeContext
{
    public int? UserId { get; set; }
    public bool IsAdmin { get; set; }
    public int? UserDepartmentId { get; set; }

    /// <summary>
    /// همه‌ی واحدهایی که کاربر به‌شون دسترسی داره
    /// (واحد خودش + زیرمجموعه‌ها، یا همه برای ادمین)
    /// </summary>
    public HashSet<int> AccessibleDepartmentIds { get; set; } = new();

    public bool IsAuthenticated => UserId.HasValue;
}