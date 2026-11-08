using TKCompass.Domain.Models;
using TKCompass.Domain.Repositories;

namespace TKCompass.Domain.Services;

public class OperationLogService : IOperationLogService
{
    private readonly IOperationLogRepository _logRepository;

    public OperationLogService(IOperationLogRepository logRepository)
    {
        _logRepository = logRepository;
    }

    public async Task LogOperationAsync(string operationType, string entityType, string entityId,
        string? beforeJson, string? afterJson, string? groupId, string? description)
    {
        var log = new OperationLog
        {
            OperationType = operationType,
            EntityType = entityType,
            EntityId = entityId,
            BeforeJson = beforeJson,
            AfterJson = afterJson,
            GroupId = groupId,
            Description = description,
            CreatedAt = DateTime.Now,
            Undone = false
        };

        await _logRepository.AddAsync(log);
        await _logRepository.SaveChangesAsync();
    }

    public async Task<IEnumerable<OperationLog>> GetRecentAsync(int count = 100)
    {
        return await _logRepository.GetRecentAsync(count);
    }

    public async Task<IEnumerable<OperationLog>> GetByGroupIdAsync(string groupId)
    {
        return await _logRepository.GetByGroupIdAsync(groupId);
    }

    public async Task PruneOldLogsAsync(int keepDays = 30, int keepMinimum = 1000)
    {
        await _logRepository.PruneOldLogsAsync(keepDays, keepMinimum);
        await _logRepository.SaveChangesAsync();
    }
}
