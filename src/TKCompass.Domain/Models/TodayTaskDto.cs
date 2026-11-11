namespace TKCompass.Domain.Models;

/// <summary>
/// 今日任务 DTO（统一跟进和履约任务）
/// </summary>
public class TodayTaskDto
{
    public long CreatorId { get; set; }
    public string Handle { get; set; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
    public string? PreferredName { get; set; }
    public string QualityLevel { get; set; } = string.Empty;
    
    /// <summary>
    /// 任务类型：FollowUp（跟进）或 Fulfillment（履约）
    /// </summary>
    public string TaskType { get; set; } = string.Empty;
    
    /// <summary>
    /// 任务描述
    /// </summary>
    public string TaskDescription { get; set; } = string.Empty;
    
    /// <summary>
    /// 任务状态：pending, completed, overdue
    /// </summary>
    public string Status { get; set; } = "pending";
    
    /// <summary>
    /// 优先级：high, medium, low
    /// </summary>
    public string Priority { get; set; } = "medium";
    
    public long? ShipmentId { get; set; }
    public string? Sku { get; set; }
    public DateOnly? SampleDate { get; set; }
    public int DaysSinceSample { get; set; }
}
