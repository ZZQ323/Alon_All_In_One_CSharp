using TKCompass.Domain.Models;
using TKCompass.Domain.Repositories;

namespace TKCompass.Domain.Services;

public class ShipmentService : IShipmentService
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly IOperationLogService _operationLogService;

    public ShipmentService(
        IShipmentRepository shipmentRepository,
        IOperationLogService operationLogService)
    {
        _shipmentRepository = shipmentRepository;
        _operationLogService = operationLogService;
    }

    public async Task<Shipment> CreateShipmentAsync(long creatorId, string trackingNumber, DateOnly sampleDate)
    {
        if (creatorId <= 0)
            throw new ArgumentException("Creator ID must be positive", nameof(creatorId));
        if (string.IsNullOrWhiteSpace(trackingNumber))
            throw new ArgumentException("Tracking number cannot be empty", nameof(trackingNumber));

        var shipment = new Shipment
        {
            CreatorId = creatorId,
            TrackingNumber = trackingNumber,
            SampleDate = sampleDate,
            RequiredVideoCount = 3,
            CreatedAt = DateTime.Now
        };

        var saved = await _shipmentRepository.AddAsync(shipment);
        await _shipmentRepository.SaveChangesAsync();

        await _operationLogService.LogOperationAsync(
            "CREATE", "Shipment", saved.Id.ToString(),
            null, System.Text.Json.JsonSerializer.Serialize(saved),
            null, $"创建寄样 #{saved.Id}（达人ID: {creatorId}）");

        return saved;
    }

    public async Task<Shipment?> GetByIdAsync(long id)
    {
        return await _shipmentRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Shipment>> GetByCreatorIdAsync(long creatorId)
    {
        return await _shipmentRepository.GetByCreatorIdAsync(creatorId);
    }

    public async Task<Shipment?> GetLatestByCreatorIdAsync(long creatorId)
    {
        return await _shipmentRepository.GetLatestByCreatorIdAsync(creatorId);
    }

    public async Task<IEnumerable<Shipment>> GetAllAsync()
    {
        return await _shipmentRepository.GetAllAsync();
    }

    public async Task<Shipment> UpdateAsync(Shipment shipment)
    {
        if (shipment.Id <= 0)
            throw new ArgumentException("Shipment ID must be positive", nameof(shipment));

        var existing = await _shipmentRepository.GetByIdAsync(shipment.Id);
        if (existing == null)
            throw new InvalidOperationException($"Shipment not found: {shipment.Id}");

        var beforeJson = System.Text.Json.JsonSerializer.Serialize(existing);
        
        await _shipmentRepository.UpdateAsync(shipment);
        await _shipmentRepository.SaveChangesAsync();

        await _operationLogService.LogOperationAsync(
            "UPDATE", "Shipment", shipment.Id.ToString(),
            beforeJson, System.Text.Json.JsonSerializer.Serialize(shipment),
            null, $"编辑寄样 #{shipment.Id}");

        return shipment;
    }

    public async Task DeleteAsync(long id)
    {
        var shipment = await _shipmentRepository.GetByIdAsync(id);
        if (shipment == null)
            throw new InvalidOperationException($"Shipment not found: {id}");

        var beforeJson = System.Text.Json.JsonSerializer.Serialize(shipment);
        
        shipment.Deleted = true;
        await _shipmentRepository.UpdateAsync(shipment);
        await _shipmentRepository.SaveChangesAsync();

        await _operationLogService.LogOperationAsync(
            "DELETE", "Shipment", id.ToString(),
            beforeJson, null,
            null, $"删除寄样 #{id}");
    }

    public async Task PermanentDeleteAsync(long id)
    {
        await _shipmentRepository.PermanentDeleteAsync(id);
        await _shipmentRepository.SaveChangesAsync();
    }
}
