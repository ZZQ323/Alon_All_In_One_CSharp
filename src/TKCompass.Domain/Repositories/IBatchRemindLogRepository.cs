using TKCompass.Domain.Models;

namespace TKCompass.Domain.Repositories;

/// <summary>
/// 批量提醒日志仓储接口
/// </summary>
public interface IBatchRemindLogRepository : IRepository<BatchRemindLog>
{
    Task<IEnumerable<BatchRemindLog>> GetRecentAsync(int count = 50);
}
