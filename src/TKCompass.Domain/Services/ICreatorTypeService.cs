using TKCompass.Domain.Models;

namespace TKCompass.Domain.Services;

/// <summary>
/// 达人类型服务接口
/// </summary>
public interface ICreatorTypeService
{
    Task<CreatorType?> GetByIdAsync(long id);
    Task<CreatorType?> GetByNameAsync(string name);
    Task<IEnumerable<CreatorType>> GetAllAsync();
    Task<IEnumerable<CreatorType>> GetAllIncludeDeletedAsync();
    Task<CreatorType> SaveAsync(CreatorType creatorType);
    Task DeleteAsync(long id);
    Task PermanentDeleteAsync(long id);
    Task<int> CountCreatorsUsingTypeAsync(long typeId);
}
