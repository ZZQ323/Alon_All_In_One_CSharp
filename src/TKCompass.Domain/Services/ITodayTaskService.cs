using TKCompass.Domain.Models;

namespace TKCompass.Domain.Services;

/// <summary>
/// 今日任务服务接口
/// </summary>
public interface ITodayTaskService
{
    /// <summary>
    /// 获取今日所有任务（跟进+履约异常）
    /// </summary>
    Task<IEnumerable<TodayTaskDto>> GetTodayTasksAsync();
    
    /// <summary>
    /// 获取今日任务总数
    /// </summary>
    Task<int> GetTodayTaskCountAsync();
    
    /// <summary>
    /// 获取高优先级任务数
    /// </summary>
    Task<int> GetHighPriorityTaskCountAsync();
}
