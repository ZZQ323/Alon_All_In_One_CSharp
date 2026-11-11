using TKCompass.Domain.Models;

namespace TKCompass.Domain.Repositories;

/// <summary>
/// 达人关系仓储接口
/// </summary>
public interface IRelationshipRepository : IRepository<Relationship>
{
    Task<Relationship?> GetByCreatorIdAsync(long creatorId);
}
