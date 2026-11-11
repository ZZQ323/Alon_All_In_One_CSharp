namespace TKCompass.Domain.Models;

/// <summary>
/// 产品代码字典
/// </summary>
public class ProductCode
{
    public long Id { get; set; }
    
    /// <summary>产品代码</summary>
    public string Code { get; set; } = string.Empty;
    
    /// <summary>产品名称</summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>显示名称（中文）</summary>
    public string? DisplayName { get; set; }
    
    /// <summary>描述</summary>
    public string? Description { get; set; }
    
    /// <summary>软删除</summary>
    public bool Deleted { get; set; } = false;
    
    /// <summary>创建时间</summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    /// <summary>更新时间</summary>
    public DateTime? UpdatedAt { get; set; }
}
