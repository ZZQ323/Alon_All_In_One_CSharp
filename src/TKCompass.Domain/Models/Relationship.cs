namespace TKCompass.Domain.Models;

/// <summary>
/// S/A/B 达人关系信息
/// </summary>
public class Relationship
{
    /// <summary>达人 ID（主键）</summary>
    public long CreatorId { get; set; }
    
    /// <summary>私人称呼</summary>
    public string? PreferredName { get; set; }
    
    /// <summary>关系备注</summary>
    public string? RelationshipNotes { get; set; }
    
    /// <summary>视频风格</summary>
    public string? VideoStyle { get; set; }
    
    /// <summary>最近联系日期</summary>
    public DateOnly? LatestContactDate { get; set; }
    
    /// <summary>下一步行动</summary>
    public string? NextAction { get; set; }
    
    /// <summary>下次跟进日期</summary>
    public DateOnly? NextFollowUpDate { get; set; }
    
    /// <summary>当前佣金</summary>
    public string? CurrentCommission { get; set; }
    
    /// <summary>更新时间</summary>
    public DateTime? UpdatedAt { get; set; }
    
    // === 导航属性 ===
    
    public Creator? Creator { get; set; }
}
