namespace TKCompass.Domain.Models;

/// <summary>
/// 批量提醒日志
/// </summary>
public class BatchRemindLog
{
    public long Id { get; set; }
    
    /// <summary>批次号</summary>
    public string? BatchNumber { get; set; }
    
    /// <summary>提醒时间</summary>
    public DateTime? RemindTime { get; set; }
    
    /// <summary>提醒用户</summary>
    public string? RemindUser { get; set; }
    
    /// <summary>提醒内容</summary>
    public string? RemindContent { get; set; }
    
    /// <summary>状态</summary>
    public string? Status { get; set; }
    
    /// <summary>提醒数量</summary>
    public int RemindCount { get; set; }
    
    /// <summary>提醒到的日期</summary>
    public DateOnly RemindToDate { get; set; }
    
    /// <summary>达人 handle 列表（JSON 数组）</summary>
    public string CreatorHandles { get; set; } = "[]";
    
    /// <summary>筛选条件描述</summary>
    public string? FilterConditions { get; set; }
    
    /// <summary>创建时间</summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    /// <summary>备注</summary>
    public string? Notes { get; set; }
}
