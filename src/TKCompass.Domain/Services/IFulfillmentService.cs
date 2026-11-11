using TKCompass.Domain.Models;

namespace TKCompass.Domain.Services;

/// <summary>
/// 履约服务接口
/// </summary>
public interface IFulfillmentService
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
    
    /// <summary>
    /// 检查达人是否已完成基础履约
    /// </summary>
    Task<bool> IsBasicFulfillmentCompleteAsync(long creatorId);
}
