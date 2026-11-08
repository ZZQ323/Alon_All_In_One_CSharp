namespace TKCompass.Domain.Models;

/// <summary>
/// 字典表（通用键值配置）
/// </summary>
public class Dictionary
{
    public long Id { get; set; }
    
    /// <summary>字典类型</summary>
    public string Type { get; set; } = string.Empty;
    
    /// <summary>字典代码</summary>
    public string Code { get; set; } = string.Empty;
    
    /// <summary>显示标签</summary>
    public string Label { get; set; } = string.Empty;
    
    /// <summary>排序</summary>
    public int SortOrder { get; set; } = 0;
    
    /// <summary>软删除</summary>
    public bool Deleted { get; set; } = false;
    
    /// <summary>创建时间</summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    /// <summary>更新时间</summary>
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
