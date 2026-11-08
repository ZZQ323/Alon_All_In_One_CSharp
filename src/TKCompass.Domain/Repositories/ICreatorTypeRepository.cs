using TKCompass.Domain.Models;

namespace TKCompass.Domain.Repositories;

/// <summary>
/// 达人类型仓储接口
/// </summary>
public interface ICreatorTypeRepository : IRepository<CreatorType>
{
    Task<CreatorType?> GetByNameAsync(string name);
    Task<IEnumerable<CreatorType>> GetAllIncludeDeletedAsync();
    Task PermanentDeleteAsync(long id);
}
