using TKCompass.Domain.Enums;

namespace TKCompass.Domain.Models;

/// <summary>
/// 达人实体
/// </summary>
public class Creator
{
    public long Id { get; set; }
    
    /// <summary>TikTok @xxx（UI 显示为"达人 ID"）</summary>
    public string Handle { get; set; } = string.Empty;
    
    /// <summary>昵称</summary>
    public string Nickname { get; set; } = string.Empty;
    
    /// <summary>S/A/B/C/D</summary>
    public string QualityLevel { get; set; } = "C";
    
    /// <summary>合作状态（状态机）</summary>
    public CooperationStatus CooperationStatus { get; set; } = CooperationStatus.Unreached;
    
    /// <summary>已交付标记（暂时不合作）</summary>
    public bool Delivered { get; set; } = false;
    
    /// <summary>已交付/删除原因</summary>
    public string? DeliveredReason { get; set; }
    
    /// <summary>达人类型 ID</summary>
    public long? CreatorTypeId { get; set; }
    
    /// <summary>今日状态</summary>
    public TodayStatus? TodayStatus { get; set; }
    
    /// <summary>今日已解决日期</summary>
    public DateOnly? TodayResolvedDate { get; set; }
    
    /// <summary>下次跟进日期</summary>
    public DateOnly? NextFollowUpDate { get; set; }
    
    /// <summary>最近联系日期</summary>
    public DateOnly? LatestContactDate { get; set; }
    
    /// <summary>软删除</summary>
    public bool Deleted { get; set; } = false;
    
    /// <summary>TikTok 主页链接</summary>
    public string? TiktokHomepageUrl { get; set; }
    
    /// <summary>语种</summary>
    public string? PreferredLanguage { get; set; }
    
    /// <summary>联系位置</summary>
    public string? ContactLocation { get; set; }
    
    // === V2.0 新增字段 ===
    
    /// <summary>国家/地区</summary>
    public string? CountryRegion { get; set; }
    
    /// <summary>内容分类</summary>
    public string? ContentCategory { get; set; }
    
    /// <summary>来源渠道</summary>
    public string? SourceChannel { get; set; }
    
    /// <summary>佣金比例</summary>
    public string? CommissionRate { get; set; }
    
    /// <summary>达人备注</summary>
    public string? CreatorNotes { get; set; }
    
    /// <summary>备注（兼容旧代码）</summary>
    public string? Notes => CreatorNotes;
    
    /// <summary>首次寄样日期</summary>
    public DateOnly? FirstSampleDate { get; set; }
    
    /// <summary>创建时间</summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>更新时间</summary>
    public DateTime UpdatedAt { get; set; }
    
    // === 已废弃字段（V27 迁移到 CreatorContact）===
    
    [Obsolete("Use CreatorContact table instead")]
    public string? Whatsapp { get; set; }
    
    [Obsolete("Use CreatorContact table instead")]
    public string? Email { get; set; }
    
    [Obsolete("Use CreatorContact table instead")]
    public string? TiktokBackendId { get; set; }
    
    [Obsolete("Use CreatorContact table instead")]
    public string? Instagram { get; set; }
    
    [Obsolete("Use CreatorContact table instead")]
    public string? OtherContact { get; set; }
    
    // === 导航属性 ===
    
    public CreatorType? CreatorType { get; set; }
    public ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
    public ICollection<CreatorContact> Contacts { get; set; } = new List<CreatorContact>();
    public ICollection<StatusFlowHistory> StatusHistories { get; set; } = new List<StatusFlowHistory>();
}
