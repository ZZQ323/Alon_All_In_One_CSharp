using TKCompass.Domain.Models;
using TKCompass.Domain.Repositories;

namespace TKCompass.Domain.Services;

/// <summary>
/// 跟进服务实现
/// </summary>
public class FollowUpService : IFollowUpService
{
    private readonly IFollowUpRepository _followUpRepository;

    public FollowUpService(IFollowUpRepository followUpRepository)
    {
        _followUpRepository = followUpRepository;
    }

    public async Task<IEnumerable<TodayFollowUpDto>> GetTodayFollowUpsAsync()
    {
        return await _followUpRepository.GetTodayFollowUpAsync();
    }

    public async Task<TodayFollowUpDto?> GetFollowUpByCreatorIdAsync(long creatorId)
    {
        var followUps = await _followUpRepository.GetTodayFollowUpAsync();
        return followUps.FirstOrDefault(f => f.CreatorId == creatorId);
    }

    public async Task MarkAsFollowedAsync(long creatorId, string? notes = null)
    {
        // TODO: 实现跟进状态更新逻辑
        // 当前仅作为占位实现
        await Task.CompletedTask;
    }

    public async Task<int> GetPendingFollowUpCountAsync()
    {
        var followUps = await _followUpRepository.GetTodayFollowUpAsync();
        return followUps.Count(f => string.IsNullOrEmpty(f.FollowUpStatus) || f.FollowUpStatus == "pending");
    }
}
