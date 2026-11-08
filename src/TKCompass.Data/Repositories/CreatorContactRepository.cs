using Microsoft.EntityFrameworkCore;
using TKCompass.Domain.Models;
using TKCompass.Domain.Repositories;

namespace TKCompass.Data.Repositories;

public class CreatorContactRepository : BaseRepository<CreatorContact>, ICreatorContactRepository
{
    public CreatorContactRepository(TKCompassDbContext context) : base(context) { }

    public async Task<IEnumerable<CreatorContact>> GetByCreatorIdAsync(long creatorId)
    {
        return await _dbSet.Where(c => c.CreatorId == creatorId).ToListAsync();
    }

    public async Task DeleteByCreatorIdAsync(long creatorId)
    {
        var contacts = await GetByCreatorIdAsync(creatorId);
        _dbSet.RemoveRange(contacts);
    }
}
