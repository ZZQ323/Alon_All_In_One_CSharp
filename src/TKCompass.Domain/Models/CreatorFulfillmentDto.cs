namespace TKCompass.Domain.Models;

/// <summary>
/// 达人履约详情 DTO（从视图读取）
/// </summary>
public class CreatorFulfillmentDto
{
    public long CreatorId { get; set; }
    public string Handle { get; set; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
    public string? PreferredName { get; set; }
    public string QualityLevel { get; set; } = string.Empty;
    public string? Tags { get; set; }
    public long? ShipmentId { get; set; }
    public string? TrackingNumber { get; set; }
    public string? Sku { get; set; }
    public string? SkuCode { get; set; }
    public DateOnly? SampleDate { get; set; }
    public string? ShipmentStatus { get; set; }
    public int RequiredVideoCount { get; set; }
    public int ValidVideoCount { get; set; }
    public int MissingVideoCount { get; set; }
    public int VideoNoCodeCount { get; set; }
    public int CodeCount { get; set; }
    public int TotalVideoCount { get; set; }
    public string? CalculatedStatus { get; set; }
    public bool HasMissingCode { get; set; }
    public decimal TotalGmv { get; set; }
}
