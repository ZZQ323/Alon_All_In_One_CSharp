namespace TKCompass.Domain.Enums;

/// <summary>
/// 合作状态机
/// </summary>
public enum CooperationStatus
{
    /// <summary>未触达</summary>
    Unreached = 0,
    
    /// <summary>已触达</summary>
    Reached = 1,
    
    /// <summary>已发送合作</summary>
    CooperationSent = 2,
    
    /// <summary>已同意合作</summary>
    CooperationAgreed = 3,
    
    /// <summary>已寄样</summary>
    SampleSent = 4
}
