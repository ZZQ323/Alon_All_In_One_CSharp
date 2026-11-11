using Microsoft.EntityFrameworkCore;
using TKCompass.Domain.Models;
using TKCompass.Domain.Repositories;

namespace TKCompass.Data.Repositories;

public class CreatorTagRepository : BaseRepository<CreatorTag>, ICreatorTagRepository
{
    public CreatorTagRepository(TKCompassDbContext context) : base(context) { }

    public async Task<IEnumerable<CreatorTag>> GetByCreatorIdAsync(long creatorId)
    {
        return await _dbSet.Where(ct => ct.CreatorId == creatorId).ToListAsync();
    }

    public async Task<IEnumerable<CreatorTag>> GetByTagIdAsync(long tagId)
    {
        return await _dbSet.Where(ct => ct.TagId == tagId).ToListAsync();
    }

    public async Task DeleteByCreatorIdAsync(long creatorId)
    {
        var tags = await GetByCreatorIdAsync(creatorId);
        _dbSet.RemoveRange(tags);
    }
}
