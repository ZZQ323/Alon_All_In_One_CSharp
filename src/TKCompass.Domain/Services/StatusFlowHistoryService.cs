using TKCompass.Domain.Models;
using TKCompass.Domain.Repositories;

namespace TKCompass.Domain.Services;

public class StatusFlowHistoryService : IStatusFlowHistoryService
{
    private readonly IStatusFlowHistoryRepository _historyRepository;

    public StatusFlowHistoryService(IStatusFlowHistoryRepository historyRepository)
    {
        _historyRepository = historyRepository;
    }

    public async Task<IEnumerable<StatusFlowHistory>> GetByCreatorIdAsync(long creatorId)
    {
        return await _historyRepository.GetByCreatorIdAsync(creatorId);
    }

    public async Task RecordHistoryAsync(long creatorId, string fromStatus, string toStatus, 
        string actionType, DateOnly actionDate, string? notes = null)
    {
        var history = new StatusFlowHistory
        {
            CreatorId = creatorId,
            FromStatus = fromStatus,
            ToStatus = toStatus,
            ActionType = actionType,
            ActionDate = actionDate,
            Notes = notes,
            CreatedAt = DateTime.Now
        };

        await _historyRepository.AddAsync(history);
        await _historyRepository.SaveChangesAsync();
    }

    public async Task DeleteByCreatorIdAsync(long creatorId)
    {
        await _historyRepository.DeleteByCreatorIdAsync(creatorId);
        await _historyRepository.SaveChangesAsync();
    }
}
