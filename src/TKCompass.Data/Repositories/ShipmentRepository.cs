using Microsoft.EntityFrameworkCore;
using TKCompass.Domain.Models;
using TKCompass.Domain.Repositories;

namespace TKCompass.Data.Repositories;

public class ShipmentRepository : BaseRepository<Shipment>, IShipmentRepository
{
    public ShipmentRepository(TKCompassDbContext context) : base(context) { }

    public override async Task<IEnumerable<Shipment>> GetAllAsync()
    {
        return await _dbSet.Where(s => !s.Deleted).ToListAsync();
    }

    public async Task<IEnumerable<Shipment>> GetByCreatorIdAsync(long creatorId)
    {
        return await _dbSet.Where(s => s.CreatorId == creatorId && !s.Deleted).ToListAsync();
    }

    public async Task<Shipment?> GetLatestByCreatorIdAsync(long creatorId)
    {
        return await _dbSet
            .Where(s => s.CreatorId == creatorId && !s.Deleted)
            .OrderByDescending(s => s.SampleDate)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Shipment>> GetAllIncludeDeletedAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task PermanentDeleteAsync(long id)
    {
        var shipment = await GetByIdAsync(id);
        if (shipment != null)
        {
            _dbSet.Remove(shipment);
        }
    }
}
