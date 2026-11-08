using TKCompass.Domain.Models;

namespace TKCompass.Domain.Repositories;

/// <summary>
/// 状态流转历史仓储接口
/// </summary>
public interface IStatusFlowHistoryRepository : IRepository<StatusFlowHistory>
{
    Task<IEnumerable<StatusFlowHistory>> GetByCreatorIdAsync(long creatorId);
    Task DeleteByCreatorIdAsync(long creatorId);
}
