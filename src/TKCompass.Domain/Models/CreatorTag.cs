namespace TKCompass.Domain.Models;

/// <summary>
/// 达人标签关联
/// </summary>
public class CreatorTag
{
    public long Id { get; set; }
    
    /// <summary>达人 ID</summary>
    public long CreatorId { get; set; }
    
    /// <summary>标签 ID</summary>
    public long TagId { get; set; }
    
    /// <summary>产品代码（可选）</summary>
    public string? ProductCode { get; set; }
    
    /// <summary>寄样 ID（可选）</summary>
    public long? ShipmentId { get; set; }
    
    /// <summary>备注</summary>
    public string? Notes { get; set; }
    
    /// <summary>创建人</summary>
    public string? CreatedBy { get; set; }
    
    /// <summary>创建时间</summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    /// <summary>更新时间</summary>
    public DateTime? UpdatedAt { get; set; }
    
    // === 导航属性 ===
    
    public Creator? Creator { get; set; }
    public TagDefinition? Tag { get; set; }
}
