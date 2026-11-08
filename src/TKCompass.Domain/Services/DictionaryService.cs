using TKCompass.Domain.Models;
using TKCompass.Domain.Repositories;

namespace TKCompass.Domain.Services;

public class DictionaryService : IDictionaryService
{
    private readonly IDictionaryRepository _dictionaryRepository;

    public DictionaryService(IDictionaryRepository dictionaryRepository)
    {
        _dictionaryRepository = dictionaryRepository;
    }

    public async Task<IEnumerable<Dictionary>> GetByTypeAsync(string type)
    {
        return await _dictionaryRepository.GetByTypeAsync(type);
    }

    public async Task<IEnumerable<Dictionary>> GetAllAsync()
    {
        return await _dictionaryRepository.GetAllAsync();
    }

    public async Task<Dictionary?> GetByTypeAndCodeAsync(string type, string code)
    {
        return await _dictionaryRepository.GetByTypeAndCodeAsync(type, code);
    }

    public async Task<Dictionary> SaveAsync(Dictionary dictionary, string? oldCode = null)
    {
        if (string.IsNullOrWhiteSpace(dictionary.Type))
            throw new ArgumentException("Type cannot be empty", nameof(dictionary));
        if (string.IsNullOrWhiteSpace(dictionary.Code))
            throw new ArgumentException("Code cannot be empty", nameof(dictionary));
        if (string.IsNullOrWhiteSpace(dictionary.Label))
            throw new ArgumentException("Label cannot be empty", nameof(dictionary));

        // TODO: 实现级联更新逻辑（当 code 变更时更新关联表）
        // 这部分需要在 Data 层实现，因为需要访问其他 Repository

        Dictionary saved;
        if (dictionary.Id > 0)
        {
            await _dictionaryRepository.UpdateAsync(dictionary);
            saved = dictionary;
        }
        else
        {
            saved = await _dictionaryRepository.AddAsync(dictionary);
        }

        await _dictionaryRepository.SaveChangesAsync();
        return saved;
    }

    public async Task DeleteAsync(string type, string code)
    {
        var dictionary = await _dictionaryRepository.GetByTypeAndCodeAsync(type, code);
        if (dictionary == null)
            throw new InvalidOperationException($"Dictionary not found: {type}/{code}");

        dictionary.Deleted = true;
        await _dictionaryRepository.UpdateAsync(dictionary);
        await _dictionaryRepository.SaveChangesAsync();
    }

    public async Task DeleteByTypeAsync(string type)
    {
        await _dictionaryRepository.DeleteByTypeAsync(type);
        await _dictionaryRepository.SaveChangesAsync();
    }
}
