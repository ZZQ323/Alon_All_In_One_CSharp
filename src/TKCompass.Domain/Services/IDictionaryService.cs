using TKCompass.Domain.Models;

namespace TKCompass.Domain.Services;

/// <summary>
/// 字典服务接口
/// </summary>
public interface IDictionaryService
{
    Task<IEnumerable<Dictionary>> GetByTypeAsync(string type);
    Task<IEnumerable<Dictionary>> GetAllAsync();
    Task<Dictionary?> GetByTypeAndCodeAsync(string type, string code);
    Task<Dictionary> SaveAsync(Dictionary dictionary, string? oldCode = null);
    Task DeleteAsync(string type, string code);
    Task DeleteByTypeAsync(string type);
}
