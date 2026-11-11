using TKCompass.Domain.Models;

namespace TKCompass.Domain.Services;

/// <summary>
/// 达人关系服务接口
/// </summary>
public interface IRelationshipService
{
    Task<Relationship?> GetByCreatorIdAsync(long creatorId);
    Task<IEnumerable<Relationship>> GetAllAsync();
    Task<Relationship> SaveAsync(Relationship relationship);
    Task DeleteAsync(long creatorId);
}
