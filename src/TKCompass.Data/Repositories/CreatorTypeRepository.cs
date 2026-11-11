using Microsoft.EntityFrameworkCore;
using TKCompass.Domain.Models;
using TKCompass.Domain.Repositories;

namespace TKCompass.Data.Repositories;

public class CreatorTypeRepository : BaseRepository<CreatorType>, ICreatorTypeRepository
{
    public CreatorTypeRepository(TKCompassDbContext context) : base(context) { }

    public override async Task<IEnumerable<CreatorType>> GetAllAsync()
    {
        return await _dbSet.Where(t => !t.Deleted).ToListAsync();
    }

    public async Task<CreatorType?> GetByNameAsync(string name)
    {
        return await _dbSet.FirstOrDefaultAsync(t => t.Name == name && !t.Deleted);
    }

    public async Task<IEnumerable<CreatorType>> GetAllIncludeDeletedAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task PermanentDeleteAsync(long id)
    {
        var type = await GetByIdAsync(id);
        if (type != null)
        {
            _dbSet.Remove(type);
        }
    }
}
