namespace TKCompass.Domain.Models;

/// <summary>
/// 寄样实体
/// </summary>
public class Shipment
{
    public long Id { get; set; }
    
    /// <summary>关联达人</summary>
    public long CreatorId { get; set; }
    
    /// <summary>快递单号</summary>
    public string? TrackingNumber { get; set; }
    
    /// <summary>产品代码（逗号分隔，如 AGTR3BK,AGTR3GD）</summary>
    public string? Sku { get; set; }
    
    /// <summary>产品名（逗号分隔，如 GTR3 黑色,GTR3 金色）</summary>
    public string? ProductName { get; set; }
    
    /// <summary>寄样日期</summary>
    public DateOnly? SampleDate { get; set; }
    
    /// <summary>要求视频数</summary>
    public int RequiredVideoCount { get; set; } = 3;
    
    /// <summary>备注</summary>
    public string? Notes { get; set; }
    
    /// <summary>手动标记完成</summary>
    public bool IsCompletedManual { get; set; } = false;
    
    /// <summary>软删除</summary>
    public bool Deleted { get; set; } = false;
    
    /// <summary>创建时间</summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    // === 导航属性 ===
    
    public Creator? Creator { get; set; }
    public ICollection<Video> Videos { get; set; } = new List<Video>();
}
