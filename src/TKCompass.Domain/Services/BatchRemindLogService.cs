using TKCompass.Domain.Models;
using TKCompass.Domain.Repositories;

namespace TKCompass.Domain.Services;

/// <summary>
/// 批量提醒日志服务实现
/// </summary>
public class BatchRemindLogService : IBatchRemindLogService
{
    private readonly IBatchRemindLogRepository _repository;

    public BatchRemindLogService(IBatchRemindLogRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<BatchRemindLog>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<BatchRemindLog?> GetByIdAsync(long id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<BatchRemindLog> CreateAsync(BatchRemindLog log)
    {
        return await _repository.AddAsync(log);
    }

    public async Task<IEnumerable<BatchRemindLog>> GetRecentAsync(int count = 50)
    {
        var all = await _repository.GetAllAsync();
        return all.OrderByDescending(l => l.CreatedAt).Take(count);
    }
}
