using TKCompass.Domain.Models;

namespace TKCompass.Domain.Services;

/// <summary>
/// 跟进服务接口
/// </summary>
public interface IFollowUpService
{
    /// <summary>
    /// 获取今日待跟进列表
    /// </summary>
    Task<IEnumerable<TodayFollowUpDto>> GetTodayFollowUpsAsync();
    
    /// <summary>
    /// 获取指定达人的跟进状态
    /// </summary>
    Task<TodayFollowUpDto?> GetFollowUpByCreatorIdAsync(long creatorId);
    
    /// <summary>
    /// 标记为已跟进
    /// </summary>
    Task MarkAsFollowedAsync(long creatorId, string? notes = null);
    
    /// <summary>
    /// 获取待跟进总数
    /// </summary>
    Task<int> GetPendingFollowUpCountAsync();
}
