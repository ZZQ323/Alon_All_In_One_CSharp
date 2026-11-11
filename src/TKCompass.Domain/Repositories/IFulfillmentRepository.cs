using TKCompass.Domain.Models;

namespace TKCompass.Domain.Repositories;

/// <summary>
/// 履约仓储接口
/// </summary>
public interface IFulfillmentRepository
{
    /// <summary>
    /// 获取所有达人履约信息
    /// </summary>
    Task<IEnumerable<CreatorFulfillmentDto>> GetAllFulfillmentAsync();

    /// <summary>
    /// 获取指定达人履约信息
    /// </summary>
    Task<CreatorFulfillmentDto?> GetFulfillmentByCreatorIdAsync(long creatorId);

    /// <summary>
    /// 获取履约异常的达人列表（缺视频或缺代码）
    /// </summary>
    Task<IEnumerable<CreatorFulfillmentDto>> GetAbnormalFulfillmentAsync();
}
