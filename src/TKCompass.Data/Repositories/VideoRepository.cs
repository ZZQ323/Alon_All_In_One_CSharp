using Microsoft.EntityFrameworkCore;
using TKCompass.Domain.Models;
using TKCompass.Domain.Repositories;

namespace TKCompass.Data.Repositories;

public class VideoRepository : BaseRepository<Video>, IVideoRepository
{
    public VideoRepository(TKCompassDbContext context) : base(context) { }

    public override async Task<IEnumerable<Video>> GetAllAsync()
    {
        return await _dbSet.Where(v => !v.Deleted).ToListAsync();
    }

    public async Task<IEnumerable<Video>> GetByCreatorIdAsync(long creatorId)
    {
        return await _dbSet.Where(v => v.CreatorId == creatorId && !v.Deleted).ToListAsync();
    }

    public async Task<IEnumerable<Video>> GetByShipmentIdAsync(long shipmentId)
    {
        return await _dbSet.Where(v => v.ShipmentId == shipmentId && !v.Deleted).ToListAsync();
    }

    public async Task<IEnumerable<Video>> GetAllIncludeDeletedAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task PermanentDeleteAsync(long id)
    {
        var video = await GetByIdAsync(id);
        if (video != null)
        {
            _dbSet.Remove(video);
        }
    }
}
