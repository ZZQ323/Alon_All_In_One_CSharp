using Microsoft.EntityFrameworkCore;
using TKCompass.Domain.Models;
using TKCompass.Domain.Repositories;

namespace TKCompass.Data.Repositories;

public class TagDefinitionRepository : BaseRepository<TagDefinition>, ITagDefinitionRepository
{
    public TagDefinitionRepository(TKCompassDbContext context) : base(context) { }

    public override async Task<IEnumerable<TagDefinition>> GetAllAsync()
    {
        return await _dbSet.Where(t => !t.Deleted).ToListAsync();
    }

    public async Task<IEnumerable<TagDefinition>> GetAllIncludeDeletedAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task PermanentDeleteAsync(long id)
    {
        var tag = await GetByIdAsync(id);
        if (tag != null)
        {
            _dbSet.Remove(tag);
        }
    }
}
