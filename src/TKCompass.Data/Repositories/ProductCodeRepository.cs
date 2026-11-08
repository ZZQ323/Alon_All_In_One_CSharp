using Microsoft.EntityFrameworkCore;
using TKCompass.Domain.Models;
using TKCompass.Domain.Repositories;

namespace TKCompass.Data.Repositories;

public class ProductCodeRepository : BaseRepository<ProductCode>, IProductCodeRepository
{
    public ProductCodeRepository(TKCompassDbContext context) : base(context) { }

    public override async Task<IEnumerable<ProductCode>> GetAllAsync()
    {
        return await _dbSet.Where(p => !p.Deleted).ToListAsync();
    }

    public async Task<ProductCode?> GetByCodeAsync(string code)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.Code == code && !p.Deleted);
    }

    public async Task<IEnumerable<ProductCode>> GetAllIncludeDeletedAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task PermanentDeleteAsync(long id)
    {
        var productCode = await GetByIdAsync(id);
        if (productCode != null)
        {
            _dbSet.Remove(productCode);
        }
    }
}
