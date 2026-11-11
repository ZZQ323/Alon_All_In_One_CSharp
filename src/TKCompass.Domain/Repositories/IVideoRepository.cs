using TKCompass.Domain.Models;

namespace TKCompass.Domain.Repositories;

/// <summary>
/// 视频仓储接口
/// </summary>
public interface IVideoRepository : IRepository<Video>
{
    Task<IEnumerable<Video>> GetByCreatorIdAsync(long creatorId);
    Task<IEnumerable<Video>> GetByShipmentIdAsync(long shipmentId);
    Task<IEnumerable<Video>> GetAllIncludeDeletedAsync();
    Task PermanentDeleteAsync(long id);
}
