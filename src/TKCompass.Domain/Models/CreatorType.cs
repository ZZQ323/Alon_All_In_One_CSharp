namespace TKCompass.Domain.Models;

/// <summary>
/// 达人类型字典
/// </summary>
public class CreatorType
{
    public long Id { get; set; }
    
    /// <summary>类型名称</summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>描述</summary>
    public string? Description { get; set; }
    
    /// <summary>软删除</summary>
    public bool Deleted { get; set; } = false;
    
    /// <summary>创建时间</summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    /// <summary>更新时间</summary>
    public DateTime? UpdatedAt { get; set; }
}
