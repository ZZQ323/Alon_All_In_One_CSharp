using Microsoft.EntityFrameworkCore;
using TKCompass.Domain.Models;
using TKCompass.Domain.Repositories;

namespace TKCompass.Data.Repositories;

public class StatusFlowHistoryRepository : BaseRepository<StatusFlowHistory>, IStatusFlowHistoryRepository
{
    public StatusFlowHistoryRepository(TKCompassDbContext context) : base(context) { }

    public async Task<IEnumerable<StatusFlowHistory>> GetByCreatorIdAsync(long creatorId)
    {
        return await _dbSet
            .Where(h => h.CreatorId == creatorId)
            .OrderByDescending(h => h.ActionDate)
            .ToListAsync();
    }

    public async Task DeleteByCreatorIdAsync(long creatorId)
    {
        var histories = await GetByCreatorIdAsync(creatorId);
        _dbSet.RemoveRange(histories);
    }
}
