namespace TKCompass.Domain.Models;

/// <summary>
/// 达人工作台 DTO（从视图读取）
/// </summary>
public class CreatorDashboardDto
{
    public long CreatorId { get; set; }
    public string Handle { get; set; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
    public string? PreferredName { get; set; }
    public string QualityLevel { get; set; } = string.Empty;
    public string? CooperationStatus { get; set; }
    public bool Delivered { get; set; }
    public int? TodayStatus { get; set; }
    public int MissingVideoCount { get; set; }
    public int MissingCodeCount { get; set; }
    public int ContinuousCount { get; set; }
    public decimal TotalGmv { get; set; }
    public DateOnly? LastContactDate { get; set; }
    public int TodayFollowUp { get; set; }
    public int ShipmentCount { get; set; }
    public int VideoCount { get; set; }
    public int ValidVideoCount { get; set; }
}
