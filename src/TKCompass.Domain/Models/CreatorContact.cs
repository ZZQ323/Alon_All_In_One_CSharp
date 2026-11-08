using TKCompass.Domain.Enums;

namespace TKCompass.Domain.Models;

/// <summary>
/// 达人联系方式（动态多值）
/// </summary>
public class CreatorContact
{
    public long Id { get; set; }
    
    /// <summary>关联达人</summary>
    public long CreatorId { get; set; }
    
    /// <summary>联系方式类型</summary>
    public ContactType ContactType { get; set; }
    
    /// <summary>联系方式值</summary>
    public string ContactValue { get; set; } = string.Empty;
    
    /// <summary>是否主要联系方式</summary>
    public bool IsPrimary { get; set; } = false;
    
    /// <summary>创建时间</summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    // === 导航属性 ===
    
    public Creator? Creator { get; set; }
}
