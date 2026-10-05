using GelatinCapsule.Application.Features.DataScopes.DTOs;

namespace GelatinCapsule.Application.Common.Interfaces;

public interface IDataScopeService
{
    /// <summary>
    /// اطلاعات scope کاربر فعلی رو برمی‌گردونه.
    /// نتیجه ۵ دقیقه cache میشه تا سرعت بالا بمونه.
    /// </summary>
    Task<DataScopeContext> GetCurrentScopeAsync(CancellationToken ct = default);

    /// <summary>
    /// cache کاربر خاص رو invalidate می‌کنه
    /// (وقتی نقش/واحد کاربر تغییر می‌کنه)
    /// </summary>
    Task InvalidateCacheAsync(int userId);
}