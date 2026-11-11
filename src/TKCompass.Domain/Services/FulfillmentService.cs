using TKCompass.Domain.Models;
using TKCompass.Domain.Repositories;

namespace TKCompass.Domain.Services;

/// <summary>
/// 履约服务实现
/// </summary>
public class FulfillmentService : IFulfillmentService
{
    private readonly IFulfillmentRepository _fulfillmentRepository;

    public FulfillmentService(IFulfillmentRepository fulfillmentRepository)
    {
        _fulfillmentRepository = fulfillmentRepository;
    }

    public async Task<IEnumerable<CreatorFulfillmentDto>> GetAllFulfillmentAsync()
    {
        return await _fulfillmentRepository.GetAllFulfillmentAsync();
    }

    public async Task<CreatorFulfillmentDto?> GetFulfillmentByCreatorIdAsync(long creatorId)
    {
        return await _fulfillmentRepository.GetFulfillmentByCreatorIdAsync(creatorId);
    }

    public async Task<IEnumerable<CreatorFulfillmentDto>> GetAbnormalFulfillmentAsync()
    {
        return await _fulfillmentRepository.GetAbnormalFulfillmentAsync();
    }

    public async Task<bool> IsBasicFulfillmentCompleteAsync(long creatorId)
    {
        var fulfillment = await _fulfillmentRepository.GetFulfillmentByCreatorIdAsync(creatorId);
        if (fulfillment == null)
            return true; // 没有寄样记录视为已完成
        
        return fulfillment.MissingVideoCount == 0 && !fulfillment.HasMissingCode;
    }
}
