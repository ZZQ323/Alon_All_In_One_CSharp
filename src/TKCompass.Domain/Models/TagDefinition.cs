namespace TKCompass.Domain.Models;

/// <summary>
/// 标签定义
/// </summary>
public class TagDefinition
{
    public long Id { get; set; }
    
    /// <summary>标签名称</summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>颜色（十六进制）</summary>
    public string? Color { get; set; }
    
    /// <summary>标签类型（creator/shipment/video）</summary>
    public string? TagType { get; set; }
    
    /// <summary>描述</summary>
    public string? Description { get; set; }
    
    /// <summary>排序</summary>
    public int SortOrder { get; set; } = 0;
    
    /// <summary>是否系统标签</summary>
    public bool IsSystem { get; set; } = false;
    
    /// <summary>软删除</summary>
    public bool Deleted { get; set; } = false;
    
    /// <summary>创建时间</summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    /// <summary>更新时间</summary>
    public DateTime? UpdatedAt { get; set; }
}
