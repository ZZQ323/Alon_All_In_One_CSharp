using TKCompass.Domain.Models;

namespace TKCompass.Domain.Repositories;

/// <summary>
/// 字典仓储接口
/// </summary>
public interface IDictionaryRepository : IRepository<Dictionary>
{
    Task<IEnumerable<Dictionary>> GetByTypeAsync(string type);
    Task<Dictionary?> GetByTypeAndCodeAsync(string type, string code);
    Task DeleteByTypeAsync(string type);
}
