using Microsoft.EntityFrameworkCore;
using TKCompass.Domain.Models;
using TKCompass.Domain.Repositories;

namespace TKCompass.Data.Repositories;

public class DictionaryRepository : BaseRepository<Dictionary>, IDictionaryRepository
{
    public DictionaryRepository(TKCompassDbContext context) : base(context) { }

    public override async Task<IEnumerable<Dictionary>> GetAllAsync()
    {
        return await _dbSet.Where(d => !d.Deleted).ToListAsync();
    }

    public async Task<IEnumerable<Dictionary>> GetByTypeAsync(string type)
    {
        return await _dbSet
            .Where(d => d.Type == type && !d.Deleted)
            .OrderBy(d => d.SortOrder)
            .ToListAsync();
    }

    public async Task<Dictionary?> GetByTypeAndCodeAsync(string type, string code)
    {
        return await _dbSet
            .FirstOrDefaultAsync(d => d.Type == type && d.Code == code && !d.Deleted);
    }

    public async Task DeleteByTypeAsync(string type)
    {
        var dictionaries = await _dbSet.Where(d => d.Type == type).ToListAsync();
        foreach (var dict in dictionaries)
        {
            dict.Deleted = true;
        }
    }
}
