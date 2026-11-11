using TKCompass.Domain.Models;

namespace TKCompass.Domain.Repositories;

/// <summary>
/// 达人标签关联仓储接口
/// </summary>
public interface ICreatorTagRepository : IRepository<CreatorTag>
{
    Task<IEnumerable<CreatorTag>> GetByCreatorIdAsync(long creatorId);
    Task<IEnumerable<CreatorTag>> GetByTagIdAsync(long tagId);
    Task DeleteByCreatorIdAsync(long creatorId);
}
