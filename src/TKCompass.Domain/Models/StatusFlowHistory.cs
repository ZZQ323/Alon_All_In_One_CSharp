namespace TKCompass.Domain.Models;

/// <summary>
/// 状态流转历史
/// </summary>
public class StatusFlowHistory
{
    public long Id { get; set; }
    
    /// <summary>达人</summary>
    public long CreatorId { get; set; }
    
    /// <summary>原状态</summary>
    public string FromStatus { get; set; } = string.Empty;
    
    /// <summary>新状态</summary>
    public string ToStatus { get; set; } = string.Empty;
    
    /// <summary>操作类型（进度推进/已提醒/已交付/重新联系）</summary>
    public string ActionType { get; set; } = string.Empty;
    
    /// <summary>操作日期</summary>
    public DateOnly ActionDate { get; set; }
    
    /// <summary>备注</summary>
    public string? Notes { get; set; }
    
    /// <summary>创建时间</summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    // === 导航属性 ===
    
    public Creator? Creator { get; set; }
}
