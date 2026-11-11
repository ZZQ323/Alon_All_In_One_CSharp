using Microsoft.EntityFrameworkCore;
using TKCompass.Domain.Models;
using TKCompass.Domain.Repositories;

namespace TKCompass.Data.Repositories;

public class FollowUpRepository : IFollowUpRepository
{
    private readonly TKCompassDbContext _context;

    public FollowUpRepository(TKCompassDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TodayFollowUpDto>> GetTodayFollowUpAsync()
    {
        // 从 v_today_follow_up 视图读取今日待跟进数据
        return await _context.TodayFollowUps
            .OrderBy(t => t.FollowUpStatus)
            .ToListAsync();
    }

    public async Task<IEnumerable<CreatorFulfillmentDto>> GetFulfillmentDataAsync(long creatorId)
    {
        // 从 v_creator_fulfillment 视图读取达人履约数据
        return await _context.CreatorFulfillments
            .Where(f => f.CreatorId == creatorId)
            .OrderByDescending(f => f.SampleDate)
            .ToListAsync();
    }
}
