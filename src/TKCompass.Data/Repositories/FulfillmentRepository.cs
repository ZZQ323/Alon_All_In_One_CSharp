using Microsoft.EntityFrameworkCore;
using TKCompass.Domain.Models;
using TKCompass.Domain.Repositories;

namespace TKCompass.Data.Repositories;

/// <summary>
/// 履约仓储实现
/// </summary>
public class FulfillmentRepository : IFulfillmentRepository
{
    private readonly TKCompassDbContext _context;

    public FulfillmentRepository(TKCompassDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CreatorFulfillmentDto>> GetAllFulfillmentAsync()
    {
        return await _context.CreatorFulfillments
            .OrderBy(f => f.CreatorId)
            .ToListAsync();
    }

    public async Task<CreatorFulfillmentDto?> GetFulfillmentByCreatorIdAsync(long creatorId)
    {
        return await _context.CreatorFulfillments
            .FirstOrDefaultAsync(f => f.CreatorId == creatorId);
    }

    public async Task<IEnumerable<CreatorFulfillmentDto>> GetAbnormalFulfillmentAsync()
    {
        return await _context.CreatorFulfillments
            .Where(f => f.HasMissingCode || f.MissingVideoCount > 0 || f.VideoNoCodeCount > 0)
            .OrderBy(f => f.CreatorId)
            .ToListAsync();
    }
}
