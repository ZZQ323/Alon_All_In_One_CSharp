namespace TKCompass.Domain.Models;

/// <summary>
/// 视频实体
/// </summary>
public class Video
{
    public long Id { get; set; }
    
    /// <summary>达人</summary>
    public long CreatorId { get; set; }
    
    /// <summary>关联寄样</summary>
    public long? ShipmentId { get; set; }
    
    /// <summary>视频链接</summary>
    public string? VideoUrl { get; set; }
    
    /// <summary>广告代码</summary>
    public string? AdCode { get; set; }
    
    /// <summary>发布时间</summary>
    public DateTime? VideoDate { get; set; }
    
    /// <summary>上传日期</summary>
    public DateTime? UploadDate { get; set; }
    
    /// <summary>视频状态</summary>
    public string? Status { get; set; }
    
    /// <summary>是否计入履约</summary>
    public bool IsEffective { get; set; } = false;
    
    /// <summary>观看数</summary>
    public int Views { get; set; } = 0;
    
    /// <summary>订单数</summary>
    public int Orders { get; set; } = 0;
    
    /// <summary>GMV</summary>
    public decimal Gmv { get; set; } = 0;
    
    /// <summary>备注</summary>
    public string? Notes { get; set; }
    
    /// <summary>软删除</summary>
    public bool Deleted { get; set; } = false;
    
    /// <summary>创建时间</summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // === 导航属性 ===
    
    public Creator? Creator { get; set; }
    public Shipment? Shipment { get; set; }
}
