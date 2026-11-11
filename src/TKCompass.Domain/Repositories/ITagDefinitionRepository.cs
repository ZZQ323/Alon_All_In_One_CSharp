using TKCompass.Domain.Models;

namespace TKCompass.Domain.Repositories;

/// <summary>
/// 标签定义仓储接口
/// </summary>
public interface ITagDefinitionRepository : IRepository<TagDefinition>
{
    Task<IEnumerable<TagDefinition>> GetAllIncludeDeletedAsync();
    Task PermanentDeleteAsync(long id);
}
