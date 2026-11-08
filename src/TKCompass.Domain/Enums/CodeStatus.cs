namespace TKCompass.Domain.Enums;

/// <summary>
/// 广告代码状态
/// </summary>
public enum CodeStatus
{
    /// <summary>无代码</summary>
    None = 0,
    
    /// <summary>等待代码</summary>
    Pending = 1,
    
    /// <summary>已收到代码</summary>
    Received = 2
}
