using Microsoft.EntityFrameworkCore;
using TKCompass.Domain.Models;
using TKCompass.Domain.Repositories;

namespace TKCompass.Data.Repositories;

/// <summary>
/// 达人关系仓储实现
/// </summary>
public class RelationshipRepository : BaseRepository<Relationship>, IRelationshipRepository
{
    public RelationshipRepository(TKCompassDbContext context) : base(context) { }

    public async Task<Relationship?> GetByCreatorIdAsync(long creatorId)
    {
        return await _dbSet
            .Include(r => r.Creator)
            .FirstOrDefaultAsync(r => r.CreatorId == creatorId);
    }

    public override async Task<IEnumerable<Relationship>> GetAllAsync()
    {
        return await _dbSet
            .Include(r => r.Creator)
            .ToListAsync();
    }

    public override async Task<Relationship?> GetByIdAsync(long id)
    {
        // Relationship 使用 CreatorId 作为主键
        return await _dbSet
            .Include(r => r.Creator)
            .FirstOrDefaultAsync(r => r.CreatorId == id);
    }

    public async Task DeleteByCreatorIdAsync(long creatorId)
    {
        var relationship = await GetByCreatorIdAsync(creatorId);
        if (relationship != null)
        {
            _dbSet.Remove(relationship);
            await _context.SaveChangesAsync();
        }
    }
}
