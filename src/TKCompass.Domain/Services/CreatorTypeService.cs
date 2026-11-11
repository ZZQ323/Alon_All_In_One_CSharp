using TKCompass.Domain.Models;
using TKCompass.Domain.Repositories;

namespace TKCompass.Domain.Services;

public class CreatorTypeService : ICreatorTypeService
{
    private readonly ICreatorTypeRepository _creatorTypeRepository;
    private readonly ICreatorRepository _creatorRepository;

    public CreatorTypeService(
        ICreatorTypeRepository creatorTypeRepository,
        ICreatorRepository creatorRepository)
    {
        _creatorTypeRepository = creatorTypeRepository;
        _creatorRepository = creatorRepository;
    }

    public async Task<CreatorType?> GetByIdAsync(long id)
    {
        return await _creatorTypeRepository.GetByIdAsync(id);
    }

    public async Task<CreatorType?> GetByNameAsync(string name)
    {
        return await _creatorTypeRepository.GetByNameAsync(name);
    }

    public async Task<IEnumerable<CreatorType>> GetAllAsync()
    {
        return await _creatorTypeRepository.GetAllAsync();
    }

    public async Task<IEnumerable<CreatorType>> GetAllIncludeDeletedAsync()
    {
        return await _creatorTypeRepository.GetAllIncludeDeletedAsync();
    }

    public async Task<CreatorType> SaveAsync(CreatorType creatorType)
    {
        if (string.IsNullOrWhiteSpace(creatorType.Name))
            throw new ArgumentException("达人类型名称不能为空", nameof(creatorType));

        CreatorType saved;
        if (creatorType.Id > 0)
        {
            creatorType.UpdatedAt = DateTime.Now;
            await _creatorTypeRepository.UpdateAsync(creatorType);
            saved = creatorType;
        }
        else
        {
            creatorType.CreatedAt = DateTime.Now;
            saved = await _creatorTypeRepository.AddAsync(creatorType);
        }

        await _creatorTypeRepository.SaveChangesAsync();
        return saved;
    }

    public async Task DeleteAsync(long id)
    {
        var creatorType = await _creatorTypeRepository.GetByIdAsync(id);
        if (creatorType == null)
            throw new InvalidOperationException($"CreatorType not found: {id}");

        creatorType.Deleted = true;
        creatorType.UpdatedAt = DateTime.Now;
        await _creatorTypeRepository.UpdateAsync(creatorType);
        await _creatorTypeRepository.SaveChangesAsync();
    }

    public async Task PermanentDeleteAsync(long id)
    {
        await _creatorTypeRepository.PermanentDeleteAsync(id);
        await _creatorTypeRepository.SaveChangesAsync();
    }

    public async Task<int> CountCreatorsUsingTypeAsync(long typeId)
    {
        var creators = await _creatorRepository.GetAllAsync();
        return creators.Count(c => c.CreatorTypeId == typeId);
    }
}
