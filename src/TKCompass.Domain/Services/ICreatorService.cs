using TKCompass.Domain.Models;
using TKCompass.Domain.Enums;

namespace TKCompass.Domain.Services;

/// <summary>
/// 达人服务接口
/// </summary>
public interface ICreatorService
{
    Task<Creator> CreateCreatorAsync(string handle, string nickname, string qualityLevel);
    Task<Creator?> GetByIdAsync(long id);
    Task<Creator?> GetByHandleAsync(string handle);
    Task<IEnumerable<Creator>> GetAllAsync();
    Task<IEnumerable<Creator>> GetAllIncludeDeletedAsync();
    Task<Creator> UpdateAsync(Creator creator);
    Task DeleteAsync(long id);
    Task PermanentDeleteAsync(long id);
    Task RestoreAsync(long id);
    Task UpdateStatusAsync(long id, CooperationStatus status);
    Task UpdateTodayStatusAsync(long id, TodayStatus? status, DateOnly? resolvedDate = null);
    Task MarkDeliveredAsync(long id, bool delivered, string? reason);
    Task ResetDailyTodayStatusAsync();
    Task<IEnumerable<CreatorDashboardDto>> GetDashboardDataAsync();
}
