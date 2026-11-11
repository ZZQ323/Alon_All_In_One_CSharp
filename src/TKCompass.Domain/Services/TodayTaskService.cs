using TKCompass.Domain.Models;

namespace TKCompass.Domain.Services;

/// <summary>
/// 今日任务服务实现
/// </summary>
public class TodayTaskService : ITodayTaskService
{
    private readonly IFollowUpService _followUpService;
    private readonly IFulfillmentService _fulfillmentService;

    public TodayTaskService(IFollowUpService followUpService, IFulfillmentService fulfillmentService)
    {
        _followUpService = followUpService;
        _fulfillmentService = fulfillmentService;
    }

    public async Task<IEnumerable<TodayTaskDto>> GetTodayTasksAsync()
    {
        var tasks = new List<TodayTaskDto>();
        
        // 1. 获取今日跟进任务
        var followUps = await _followUpService.GetTodayFollowUpsAsync();
        foreach (var fu in followUps)
        {
            tasks.Add(new TodayTaskDto
            {
                CreatorId = fu.CreatorId,
                Handle = fu.Handle,
                Nickname = fu.Nickname,
                PreferredName = fu.PreferredName,
                QualityLevel = fu.QualityLevel,
                TaskType = "FollowUp",
                TaskDescription = $"跟进达人 {fu.Nickname}（寄样 {fu.DaysSinceSample} 天）",
                Status = string.IsNullOrEmpty(fu.FollowUpStatus) || fu.FollowUpStatus == "pending" ? "pending" : "completed",
                Priority = fu.DaysSinceSample > 7 ? "high" : fu.DaysSinceSample > 3 ? "medium" : "low",
                ShipmentId = fu.ShipmentId,
                Sku = fu.Sku,
                SampleDate = fu.SampleDate,
                DaysSinceSample = fu.DaysSinceSample
            });
        }
        
        // 2. 获取履约异常任务
        var abnormalFulfillments = await _fulfillmentService.GetAbnormalFulfillmentAsync();
        foreach (var af in abnormalFulfillments)
        {
            // 避免重复添加（如果已经在跟进任务中）
            if (!tasks.Any(t => t.CreatorId == af.CreatorId && t.TaskType == "Fulfillment"))
            {
                var description = af.MissingVideoCount > 0 ? $"缺 {af.MissingVideoCount} 个视频" : "";
                if (af.HasMissingCode)
                {
                    description += string.IsNullOrEmpty(description) ? $"缺 {af.VideoNoCodeCount} 个代码" : $"，缺 {af.VideoNoCodeCount} 个代码";
                }
                
                tasks.Add(new TodayTaskDto
                {
                    CreatorId = af.CreatorId,
                    Handle = af.Handle,
                    Nickname = af.Nickname,
                    PreferredName = af.PreferredName,
                    QualityLevel = af.QualityLevel,
                    TaskType = "Fulfillment",
                    TaskDescription = $"履约异常：{description}",
                    Status = "pending",
                    Priority = af.MissingVideoCount > 2 || af.VideoNoCodeCount > 2 ? "high" : "medium",
                    ShipmentId = af.ShipmentId,
                    Sku = af.Sku,
                    SampleDate = af.SampleDate
                });
            }
        }
        
        // 按优先级排序：high > medium > low
        return tasks.OrderByDescending(t => t.Priority == "high" ? 3 : t.Priority == "medium" ? 2 : 1);
    }

    public async Task<int> GetTodayTaskCountAsync()
    {
        var tasks = await GetTodayTasksAsync();
        return tasks.Count();
    }

    public async Task<int> GetHighPriorityTaskCountAsync()
    {
        var tasks = await GetTodayTasksAsync();
        return tasks.Count(t => t.Priority == "high");
    }
}
