using TKCompass.Domain.Models;

namespace TKCompass.Domain.Repositories;

/// <summary>
/// 操作日志仓储接口
/// </summary>
public interface IOperationLogRepository : IRepository<OperationLog>
{
    Task<IEnumerable<OperationLog>> GetRecentAsync(int count = 100);
    Task<IEnumerable<OperationLog>> GetByGroupIdAsync(string groupId);
    Task<OperationLog?> GetLastUndoableAsync();
    Task PruneOldLogsAsync(int keepDays = 30, int keepMinimum = 1000);
}
