using TKCompass.Domain.Models;

namespace TKCompass.Domain.Services;

/// <summary>
/// 状态流转历史服务接口
/// </summary>
public interface IStatusFlowHistoryService
{
    Task<IEnumerable<StatusFlowHistory>> GetByCreatorIdAsync(long creatorId);
    Task RecordHistoryAsync(long creatorId, string fromStatus, string toStatus, string actionType, DateOnly actionDate, string? notes = null);
    Task DeleteByCreatorIdAsync(long creatorId);
}
