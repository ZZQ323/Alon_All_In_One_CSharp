using TKCompass.Domain.Models;

namespace TKCompass.Domain.Services;

/// <summary>
/// 寄样服务接口
/// </summary>
public interface IShipmentService
{
    Task<Shipment> CreateShipmentAsync(long creatorId, string trackingNumber, DateOnly sampleDate);
    Task<Shipment?> GetByIdAsync(long id);
    Task<IEnumerable<Shipment>> GetByCreatorIdAsync(long creatorId);
    Task<Shipment?> GetLatestByCreatorIdAsync(long creatorId);
    Task<IEnumerable<Shipment>> GetAllAsync();
    Task<Shipment> UpdateAsync(Shipment shipment);
    Task DeleteAsync(long id);
    Task PermanentDeleteAsync(long id);
}
