using TKCompass.Domain.Models;
using TKCompass.Domain.Enums;

namespace TKCompass.Domain.Repositories;

/// <summary>
/// 达人仓储接口
/// </summary>
public interface ICreatorRepository : IRepository<Creator>
{
    Task<Creator?> GetByHandleAsync(string handle);
    Task<IEnumerable<Creator>> GetAllIncludeDeletedAsync();
    Task<IEnumerable<Creator>> FindByStatusAsync(CooperationStatus status);
    Task<IEnumerable<Creator>> FindDeliveredAsync();
    Task<IEnumerable<Creator>> FindDeletedAsync();
    Task PermanentDeleteAsync(long id);
    Task RestoreAsync(long id);
    Task<IEnumerable<CreatorDashboardDto>> GetDashboardDataAsync();
    Task UpdateStatusAsync(long id, CooperationStatus status);
    Task UpdateTodayStatusAsync(long id, TodayStatus? status, DateOnly? resolvedDate = null);
    Task MarkDeliveredAsync(long id, bool delivered, string? reason);
    Task ResetDailyTodayStatusAsync();
}
