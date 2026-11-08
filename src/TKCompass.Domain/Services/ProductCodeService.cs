using TKCompass.Domain.Models;
using TKCompass.Domain.Repositories;

namespace TKCompass.Domain.Services;

public class ProductCodeService : IProductCodeService
{
    private readonly IProductCodeRepository _productCodeRepository;
    private readonly IShipmentRepository _shipmentRepository;

    public ProductCodeService(
        IProductCodeRepository productCodeRepository,
        IShipmentRepository shipmentRepository)
    {
        _productCodeRepository = productCodeRepository;
        _shipmentRepository = shipmentRepository;
    }

    public async Task<ProductCode?> GetByIdAsync(long id)
    {
        return await _productCodeRepository.GetByIdAsync(id);
    }

    public async Task<ProductCode?> GetByCodeAsync(string code)
    {
        return await _productCodeRepository.GetByCodeAsync(code);
    }

    public async Task<IEnumerable<ProductCode>> GetAllAsync()
    {
        return await _productCodeRepository.GetAllAsync();
    }

    public async Task<IEnumerable<ProductCode>> GetAllIncludeDeletedAsync()
    {
        return await _productCodeRepository.GetAllIncludeDeletedAsync();
    }

    public async Task<ProductCode> SaveAsync(ProductCode productCode)
    {
        if (string.IsNullOrWhiteSpace(productCode.Code))
            throw new ArgumentException("产品代码不能为空", nameof(productCode));
        if (string.IsNullOrWhiteSpace(productCode.Name))
            throw new ArgumentException("产品名称不能为空", nameof(productCode));

        // TODO: 实现级联更新逻辑（当 code 或 name 变更时更新 shipments 表）
        // 这部分需要在 Data 层实现

        ProductCode saved;
        if (productCode.Id > 0)
        {
            productCode.UpdatedAt = DateTime.Now;
            await _productCodeRepository.UpdateAsync(productCode);
            saved = productCode;
        }
        else
        {
            productCode.CreatedAt = DateTime.Now;
            saved = await _productCodeRepository.AddAsync(productCode);
        }

        await _productCodeRepository.SaveChangesAsync();
        return saved;
    }

    public async Task DeleteAsync(long id)
    {
        var productCode = await _productCodeRepository.GetByIdAsync(id);
        if (productCode == null)
            throw new InvalidOperationException($"ProductCode not found: {id}");

        productCode.Deleted = true;
        productCode.UpdatedAt = DateTime.Now;
        await _productCodeRepository.UpdateAsync(productCode);
        await _productCodeRepository.SaveChangesAsync();
    }

    public async Task PermanentDeleteAsync(long id)
    {
        await _productCodeRepository.PermanentDeleteAsync(id);
        await _productCodeRepository.SaveChangesAsync();
    }

    public async Task<int> CountShipmentsUsingCodeAsync(long codeId)
    {
        var productCode = await _productCodeRepository.GetByIdAsync(codeId);
        if (productCode == null)
            return 0;

        var shipments = await _shipmentRepository.GetAllAsync();
        return shipments.Count(s => s.Sku?.Contains(productCode.Code) == true);
    }
}
