using Microsoft.EntityFrameworkCore;
using TKCompass.Domain.Enums;
using TKCompass.Domain.Models;
using TKCompass.Domain.Repositories;

namespace TKCompass.Data.Repositories;

public class CreatorRepository : BaseRepository<Creator>, ICreatorRepository
{
    public CreatorRepository(TKCompassDbContext context) : base(context) { }

    public override async Task<IEnumerable<Creator>> GetAllAsync()
    {
        return await _dbSet.Where(c => !c.Deleted).ToListAsync();
    }

    public async Task<Creator?> GetByHandleAsync(string handle)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.Handle == handle && !c.Deleted);
    }

    public async Task<IEnumerable<Creator>> GetAllIncludeDeletedAsync()
    {
        return await _dbSet
            .Include(c => c.Shipments)
            .ToListAsync();
    }

    public async Task<IEnumerable<Creator>> FindByStatusAsync(CooperationStatus status)
    {
        return await _dbSet.Where(c => c.CooperationStatus == status && !c.Deleted).ToListAsync();
    }

    public async Task<IEnumerable<Creator>> FindDeliveredAsync()
    {
        return await _dbSet.Where(c => c.Delivered && !c.Deleted).ToListAsync();
    }

    public async Task<IEnumerable<Creator>> FindDeletedAsync()
    {
        return await _dbSet.Where(c => c.Deleted).ToListAsync();
    }

    public async Task PermanentDeleteAsync(long id)
    {
        var creator = await GetByIdAsync(id);
        if (creator != null)
        {
            _dbSet.Remove(creator);
        }
    }

    public async Task RestoreAsync(long id)
    {
        var creator = await _dbSet.IgnoreQueryFilters().FirstOrDefaultAsync(c => c.Id == id);
        if (creator != null)
        {
            creator.Deleted = false;
        }
    }

    public async Task<IEnumerable<CreatorDashboardDto>> GetDashboardDataAsync()
    {
        // 从 v_creator_dashboard 视图直接读取数据
        return await _context.CreatorDashboards
            .OrderByDescending(d => d.TotalGmv)
            .ToListAsync();
    }

    public async Task UpdateStatusAsync(long id, CooperationStatus status)
    {
        var creator = await GetByIdAsync(id);
        if (creator != null)
        {
            creator.CooperationStatus = status;
        }
    }

    public async Task UpdateTodayStatusAsync(long id, TodayStatus? status, DateOnly? resolvedDate = null)
    {
        var creator = await GetByIdAsync(id);
        if (creator != null)
        {
            creator.TodayStatus = status;
            if (resolvedDate.HasValue)
            {
                creator.TodayResolvedDate = resolvedDate.Value;
            }
        }
    }

    public async Task MarkDeliveredAsync(long id, bool delivered, string? reason)
    {
        var creator = await GetByIdAsync(id);
        if (creator != null)
        {
            creator.Delivered = delivered;
            creator.DeliveredReason = reason;
        }
    }

    public async Task ResetDailyTodayStatusAsync()
    {
        var creators = await _dbSet.Where(c => c.TodayStatus != null && !c.Deleted).ToListAsync();
        foreach (var creator in creators)
        {
            creator.TodayStatus = null;
        }
    }
}
