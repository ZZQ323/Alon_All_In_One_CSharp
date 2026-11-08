using TKCompass.Domain.Models;

namespace TKCompass.Domain.Services;

/// <summary>
/// 视频服务接口
/// </summary>
public interface IVideoService
{
    Task<Video> CreateVideoAsync(long creatorId, string videoUrl);
    Task<Video?> GetByIdAsync(long id);
    Task<IEnumerable<Video>> GetByCreatorIdAsync(long creatorId);
    Task<IEnumerable<Video>> GetByShipmentIdAsync(long shipmentId);
    Task<IEnumerable<Video>> GetAllAsync();
    Task<Video> UpdateAsync(Video video);
    Task DeleteAsync(long id);
    Task PermanentDeleteAsync(long id);
}
