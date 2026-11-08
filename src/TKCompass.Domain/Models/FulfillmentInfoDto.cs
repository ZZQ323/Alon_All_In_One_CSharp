namespace TKCompass.Domain.Models;

/// <summary>
/// 履约状态 DTO（从视图读取）
/// </summary>
public class FulfillmentInfoDto
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
}
