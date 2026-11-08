using TKCompass.Domain.Models;
using TKCompass.Domain.Enums;
using TKCompass.Domain.Repositories;

namespace TKCompass.Domain.Services;

/// <summary>
/// 达人服务实现
/// </summary>
public class CreatorService : ICreatorService
{
    private readonly ICreatorRepository _creatorRepository;
    private readonly IStatusFlowHistoryService _statusFlowHistoryService;
    private readonly IOperationLogService _operationLogService;

    public CreatorService(
        ICreatorRepository creatorRepository,
        IStatusFlowHistoryService statusFlowHistoryService,
        IOperationLogService operationLogService)
    {
        _creatorRepository = creatorRepository;
        _statusFlowHistoryService = statusFlowHistoryService;
        _operationLogService = operationLogService;
    }

    public async Task<Creator> CreateCreatorAsync(string handle, string nickname, string qualityLevel)
    {
        if (string.IsNullOrWhiteSpace(handle))
            throw new ArgumentException("Handle cannot be empty", nameof(handle));
        if (string.IsNullOrWhiteSpace(nickname))
            throw new ArgumentException("Nickname cannot be empty", nameof(nickname));
        if (string.IsNullOrWhiteSpace(qualityLevel))
            throw new ArgumentException("Quality level cannot be empty", nameof(qualityLevel));

        var existing = await _creatorRepository.GetByHandleAsync(handle);
        if (existing != null)
            throw new InvalidOperationException($"账号ID已存在: {handle}");

        var creator = new Creator
        {
            Handle = handle,
            Nickname = nickname,
            QualityLevel = qualityLevel,
            CooperationStatus = CooperationStatus.Reached, // 新建后立即流转为"已触达"
            LatestContactDate = DateOnly.FromDateTime(DateTime.Now),
            TiktokHomepageUrl = $"https://www.tiktok.com/@{handle}",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        var saved = await _creatorRepository.AddAsync(creator);
        await _creatorRepository.SaveChangesAsync();

        // 记录操作日志
        await _operationLogService.LogOperationAsync(
            "CREATE", "Creator", saved.Id.ToString(),
            null, System.Text.Json.JsonSerializer.Serialize(saved),
            null, $"创建达人 @{saved.Handle}");

        return saved;
    }

    public async Task<Creator?> GetByIdAsync(long id)
    {
        return await _creatorRepository.GetByIdAsync(id);
    }

    public async Task<Creator?> GetByHandleAsync(string handle)
    {
        return await _creatorRepository.GetByHandleAsync(handle);
    }

    public async Task<IEnumerable<Creator>> GetAllAsync()
    {
        return await _creatorRepository.GetAllAsync();
    }

    public async Task<IEnumerable<Creator>> GetAllIncludeDeletedAsync()
    {
        return await _creatorRepository.GetAllIncludeDeletedAsync();
    }

    public async Task<Creator> UpdateAsync(Creator creator)
    {
        if (creator.Id <= 0)
            throw new ArgumentException("Creator ID must be positive", nameof(creator));

        var existing = await _creatorRepository.GetByIdAsync(creator.Id);
        if (existing == null)
            throw new InvalidOperationException($"Creator not found: {creator.Id}");

        var beforeJson = System.Text.Json.JsonSerializer.Serialize(existing);
        
        creator.UpdatedAt = DateTime.Now;
        await _creatorRepository.UpdateAsync(creator);
        await _creatorRepository.SaveChangesAsync();

        await _operationLogService.LogOperationAsync(
            "UPDATE", "Creator", creator.Id.ToString(),
            beforeJson, System.Text.Json.JsonSerializer.Serialize(creator),
            null, $"编辑达人 #{creator.Id}");

        return creator;
    }

    public async Task DeleteAsync(long id)
    {
        var creator = await _creatorRepository.GetByIdAsync(id);
        if (creator == null)
            throw new InvalidOperationException($"Creator not found: {id}");

        var beforeJson = System.Text.Json.JsonSerializer.Serialize(creator);
        
        creator.Deleted = true;
        await _creatorRepository.UpdateAsync(creator);
        await _creatorRepository.SaveChangesAsync();

        await _operationLogService.LogOperationAsync(
            "DELETE", "Creator", id.ToString(),
            beforeJson, null,
            null, $"删除达人 #{id}");
    }

    public async Task PermanentDeleteAsync(long id)
    {
        await _creatorRepository.PermanentDeleteAsync(id);
        await _creatorRepository.SaveChangesAsync();
    }

    public async Task RestoreAsync(long id)
    {
        await _creatorRepository.RestoreAsync(id);
        await _creatorRepository.SaveChangesAsync();
    }

    public async Task UpdateStatusAsync(long id, CooperationStatus status)
    {
        var creator = await _creatorRepository.GetByIdAsync(id);
        if (creator == null)
            throw new InvalidOperationException($"Creator not found: {id}");

        var oldStatus = creator.CooperationStatus;
        creator.CooperationStatus = status;
        creator.UpdatedAt = DateTime.Now;

        await _creatorRepository.UpdateAsync(creator);
        await _creatorRepository.SaveChangesAsync();

        // 记录状态流转历史
        await _statusFlowHistoryService.RecordHistoryAsync(
            id, oldStatus.ToString(), status.ToString(),
            "状态推进", DateOnly.FromDateTime(DateTime.Now));
    }

    public async Task UpdateTodayStatusAsync(long id, TodayStatus? status, DateOnly? resolvedDate = null)
    {
        await _creatorRepository.UpdateTodayStatusAsync(id, status, resolvedDate);
        await _creatorRepository.SaveChangesAsync();
    }

    public async Task MarkDeliveredAsync(long id, bool delivered, string? reason)
    {
        await _creatorRepository.MarkDeliveredAsync(id, delivered, reason);
        await _creatorRepository.SaveChangesAsync();
    }

    public async Task ResetDailyTodayStatusAsync()
    {
        await _creatorRepository.ResetDailyTodayStatusAsync();
        await _creatorRepository.SaveChangesAsync();
    }

    public async Task<IEnumerable<CreatorDashboardDto>> GetDashboardDataAsync()
    {
        return await _creatorRepository.GetDashboardDataAsync();
    }
}
