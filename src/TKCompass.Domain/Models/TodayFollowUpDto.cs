namespace TKCompass.Domain.Models;

/// <summary>
/// 今日跟进 DTO（从视图读取）
/// </summary>
public class TodayFollowUpDto
{
    public long CreatorId { get; set; }
    public string Handle { get; set; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
    public string? PreferredName { get; set; }
    public string QualityLevel { get; set; } = string.Empty;
    public int ShipmentCount { get; set; }
    public int VideoCount { get; set; }
    public int CodeCount { get; set; }
    public string FulfillmentStatus { get; set; } = string.Empty;
    public string StatusColor { get; set; } = string.Empty;
    public long? ShipmentId { get; set; }
    public string? Sku { get; set; }
    public DateOnly? SampleDate { get; set; }
    public int DaysSinceSample { get; set; }
    public string FollowUpStatus { get; set; } = string.Empty;
}
