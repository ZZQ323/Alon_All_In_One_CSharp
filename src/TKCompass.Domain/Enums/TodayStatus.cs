namespace TKCompass.Domain.Enums;

/// <summary>
/// 今日任务状态
/// </summary>
public enum TodayStatus
{
    /// <summary>待处理</summary>
    Pending = 0,
    
    /// <summary>已提醒</summary>
    Reminded = 1,
    
    /// <summary>已解决</summary>
    Resolved = 2
}
