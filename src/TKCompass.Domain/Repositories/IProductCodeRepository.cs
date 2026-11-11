using TKCompass.Domain.Models;

namespace TKCompass.Domain.Repositories;

/// <summary>
/// 产品代码仓储接口
/// </summary>
public interface IProductCodeRepository : IRepository<ProductCode>
{
    Task<ProductCode?> GetByCodeAsync(string code);
    Task<IEnumerable<ProductCode>> GetAllIncludeDeletedAsync();
    Task PermanentDeleteAsync(long id);
}
