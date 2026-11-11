namespace TKCompass.Domain.Enums;

/// <summary>
/// 跟进状态（按寄样粒度）
/// </summary>
public enum FollowUpStatus
{
    /// <summary>待跟进</summary>
    Pending = 0,
    
    /// <summary>已确认</summary>
    Acknowledged = 1,
    
    /// <summary>已完成</summary>
    Completed = 2
}
