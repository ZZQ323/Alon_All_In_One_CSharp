using TKCompass.Domain.Models;

namespace TKCompass.Domain.Repositories;

/// <summary>
/// 寄样仓储接口
/// </summary>
public interface IShipmentRepository : IRepository<Shipment>
{
    Task<IEnumerable<Shipment>> GetByCreatorIdAsync(long creatorId);
    Task<Shipment?> GetLatestByCreatorIdAsync(long creatorId);
    Task<IEnumerable<Shipment>> GetAllIncludeDeletedAsync();
    Task PermanentDeleteAsync(long id);
}
