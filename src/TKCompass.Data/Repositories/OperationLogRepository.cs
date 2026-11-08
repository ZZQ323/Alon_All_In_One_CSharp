using Microsoft.EntityFrameworkCore;
using TKCompass.Domain.Models;
using TKCompass.Domain.Repositories;

namespace TKCompass.Data.Repositories;

public class OperationLogRepository : BaseRepository<OperationLog>, IOperationLogRepository
{
    public OperationLogRepository(TKCompassDbContext context) : base(context) { }

    public async Task<IEnumerable<OperationLog>> GetRecentAsync(int count = 100)
    {
        return await _dbSet
            .OrderByDescending(l => l.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<OperationLog>> GetByGroupIdAsync(string groupId)
    {
        return await _dbSet
            .Where(l => l.GroupId == groupId)
            .OrderBy(l => l.CreatedAt)
            .ToListAsync();
    }

    public async Task<OperationLog?> GetLastUndoableAsync()
    {
        return await _dbSet
            .Where(l => !l.Undone)
            .OrderByDescending(l => l.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task PruneOldLogsAsync(int keepDays = 30, int keepMinimum = 1000)
    {
        var cutoffDate = DateTime.Now.AddDays(-keepDays);
        var totalCount = await _dbSet.CountAsync();
        
        if (totalCount <= keepMinimum)
            return;

        var oldLogs = await _dbSet
            .Where(l => l.CreatedAt < cutoffDate)
            .OrderBy(l => l.CreatedAt)
            .Take(totalCount - keepMinimum)
            .ToListAsync();

        _dbSet.RemoveRange(oldLogs);
    }
}
