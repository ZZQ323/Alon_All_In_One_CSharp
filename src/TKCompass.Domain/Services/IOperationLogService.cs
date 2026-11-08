using TKCompass.Domain.Models;

namespace TKCompass.Domain.Services;

/// <summary>
/// 操作日志服务接口
/// </summary>
public interface IOperationLogService
{
    Task LogOperationAsync(string operationType, string entityType, string entityId, string? beforeJson, string? afterJson, string? groupId, string? description);
    Task<IEnumerable<OperationLog>> GetRecentAsync(int count = 100);
    Task<IEnumerable<OperationLog>> GetByGroupIdAsync(string groupId);
    Task PruneOldLogsAsync(int keepDays = 30, int keepMinimum = 1000);
}
