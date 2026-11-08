using TKCompass.Domain.Models;
using TKCompass.Domain.Enums;

namespace TKCompass.Domain.Services;

/// <summary>
/// 达人联系方式服务接口
/// </summary>
public interface ICreatorContactService
{
    Task<IEnumerable<CreatorContact>> GetAllAsync();
    Task<IEnumerable<CreatorContact>> GetByCreatorIdAsync(long creatorId);
    Task<IEnumerable<CreatorContact>> GetByCreatorIdAndTypeAsync(long creatorId, ContactType type);
    Task<CreatorContact> AddContactAsync(long creatorId, ContactType type, string value);
    Task UpdateAsync(CreatorContact contact);
    Task DeleteAsync(long contactId);
    Task DeleteAllByCreatorIdAsync(long creatorId);
}
