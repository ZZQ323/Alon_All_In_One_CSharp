using TKCompass.Domain.Models;

namespace TKCompass.Domain.Repositories;

/// <summary>
/// 达人联系方式仓储接口
/// </summary>
public interface ICreatorContactRepository : IRepository<CreatorContact>
{
    Task<IEnumerable<CreatorContact>> GetByCreatorIdAsync(long creatorId);
    Task DeleteByCreatorIdAsync(long creatorId);
}
