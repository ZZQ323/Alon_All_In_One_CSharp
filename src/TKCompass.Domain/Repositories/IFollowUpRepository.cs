using TKCompass.Domain.Models;

namespace TKCompass.Domain.Repositories;

/// <summary>
/// 跟进与履约仓储接口
/// </summary>
public interface IFollowUpRepository
{
    /// <summary>
    /// 获取今日待跟进列表
    /// </summary>
    Task<IEnumerable<TodayFollowUpDto>> GetTodayFollowUpAsync();
    
    /// <summary>
    /// 获取达人履约数据
    /// </summary>
    /// <param name="creatorId">达人ID</param>
    Task<IEnumerable<CreatorFulfillmentDto>> GetFulfillmentDataAsync(long creatorId);
}
