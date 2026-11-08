using TKCompass.Domain.Models;
using TKCompass.Domain.Repositories;

namespace TKCompass.Domain.Services;

/// <summary>
/// 达人关系服务实现
/// </summary>
public class RelationshipService : IRelationshipService
{
    private readonly IRelationshipRepository _repository;

    public RelationshipService(IRelationshipRepository repository)
    {
        _repository = repository;
    }

    public async Task<Relationship?> GetByCreatorIdAsync(long creatorId)
    {
        return await _repository.GetByCreatorIdAsync(creatorId);
    }

    public async Task<IEnumerable<Relationship>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Relationship> SaveAsync(Relationship relationship)
    {
        var existing = await _repository.GetByCreatorIdAsync(relationship.CreatorId);
        if (existing == null)
        {
            return await _repository.AddAsync(relationship);
        }
        else
        {
            existing.PreferredName = relationship.PreferredName;
            existing.RelationshipNotes = relationship.RelationshipNotes;
            existing.VideoStyle = relationship.VideoStyle;
            existing.LatestContactDate = relationship.LatestContactDate;
            existing.NextAction = relationship.NextAction;
            existing.NextFollowUpDate = relationship.NextFollowUpDate;
            existing.CurrentCommission = relationship.CurrentCommission;
            existing.UpdatedAt = DateTime.Now;
            await _repository.UpdateAsync(existing);
            return existing;
        }
    }

    public async Task DeleteAsync(long creatorId)
    {
        var relationship = await _repository.GetByCreatorIdAsync(creatorId);
        if (relationship != null)
        {
            await _repository.DeleteAsync(relationship);
        }
    }
}
