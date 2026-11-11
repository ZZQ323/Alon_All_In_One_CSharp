using TKCompass.Domain.Models;

namespace TKCompass.Domain.Services;

/// <summary>
/// 批量提醒日志服务接口
/// </summary>
public interface IBatchRemindLogService
{
    Task<IEnumerable<BatchRemindLog>> GetAllAsync();
    Task<BatchRemindLog?> GetByIdAsync(long id);
    Task<BatchRemindLog> CreateAsync(BatchRemindLog log);
    Task<IEnumerable<BatchRemindLog>> GetRecentAsync(int count = 50);
}
