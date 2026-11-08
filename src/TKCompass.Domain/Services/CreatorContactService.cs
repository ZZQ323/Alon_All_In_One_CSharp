using TKCompass.Domain.Models;
using TKCompass.Domain.Enums;
using TKCompass.Domain.Repositories;

namespace TKCompass.Domain.Services;

public class CreatorContactService : ICreatorContactService
{
    private readonly ICreatorContactRepository _repository;

    public CreatorContactService(ICreatorContactRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<CreatorContact>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<IEnumerable<CreatorContact>> GetByCreatorIdAsync(long creatorId)
    {
        return await _repository.GetByCreatorIdAsync(creatorId);
    }

    public async Task<IEnumerable<CreatorContact>> GetByCreatorIdAndTypeAsync(long creatorId, ContactType type)
    {
        var all = await _repository.GetByCreatorIdAsync(creatorId);
        return all.Where(c => c.ContactType == type);
    }

    public async Task<CreatorContact> AddContactAsync(long creatorId, ContactType type, string value)
    {
        var contact = new CreatorContact
        {
            CreatorId = creatorId,
            ContactType = type,
            ContactValue = value
        };
        return await _repository.AddAsync(contact);
    }

    public async Task UpdateAsync(CreatorContact contact)
    {
        await _repository.UpdateAsync(contact);
    }

    public async Task DeleteAsync(long contactId)
    {
        var contact = await _repository.GetByIdAsync(contactId);
        if (contact != null)
        {
            await _repository.DeleteAsync(contact);
        }
    }

    public async Task DeleteAllByCreatorIdAsync(long creatorId)
    {
        var contacts = await _repository.GetByCreatorIdAsync(creatorId);
        foreach (var contact in contacts)
        {
            await _repository.DeleteAsync(contact);
        }
    }
}
