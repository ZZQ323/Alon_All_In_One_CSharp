using Microsoft.EntityFrameworkCore;
using TKCompass.Domain.Models;
using TKCompass.Domain.Repositories;

namespace TKCompass.Data.Repositories;

public class BatchRemindLogRepository : BaseRepository<BatchRemindLog>, IBatchRemindLogRepository
{
    public BatchRemindLogRepository(TKCompassDbContext context) : base(context) { }

    public async Task<IEnumerable<BatchRemindLog>> GetRecentAsync(int count = 50)
    {
        return await _dbSet
            .OrderByDescending(l => l.CreatedAt)
            .Take(count)
            .ToListAsync();
    }
}
