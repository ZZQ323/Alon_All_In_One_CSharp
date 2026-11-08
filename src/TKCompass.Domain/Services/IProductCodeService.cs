using TKCompass.Domain.Models;

namespace TKCompass.Domain.Services;

/// <summary>
/// 产品代码服务接口
/// </summary>
public interface IProductCodeService
{
    Task<ProductCode?> GetByIdAsync(long id);
    Task<ProductCode?> GetByCodeAsync(string code);
    Task<IEnumerable<ProductCode>> GetAllAsync();
    Task<IEnumerable<ProductCode>> GetAllIncludeDeletedAsync();
    Task<ProductCode> SaveAsync(ProductCode productCode);
    Task DeleteAsync(long id);
    Task PermanentDeleteAsync(long id);
    Task<int> CountShipmentsUsingCodeAsync(long codeId);
}
