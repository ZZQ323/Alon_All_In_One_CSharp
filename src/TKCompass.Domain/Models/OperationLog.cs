namespace TKCompass.Domain.Models;

/// <summary>
/// 操作日志（撤销系统基础）
/// </summary>
public class OperationLog
{
    public long Id { get; set; }
    
    /// <summary>操作类型（CREATE/UPDATE/DELETE/BATCH_IMPORT/STATE_TRANSITION/UNDO）</summary>
    public string OperationType { get; set; } = string.Empty;
    
    /// <summary>实体类型（Creator/Shipment/Video 等）</summary>
    public string EntityType { get; set; } = string.Empty;
    
    /// <summary>实体 ID</summary>
    public string EntityId { get; set; } = string.Empty;
    
    /// <summary>操作前的 JSON 快照</summary>
    public string? BeforeJson { get; set; }
    
    /// <summary>操作后的 JSON 快照</summary>
    public string? AfterJson { get; set; }
    
    /// <summary>变更前数据（兼容旧代码）</summary>
    public string? BeforeData => BeforeJson;
    
    /// <summary>变更后数据（兼容旧代码）</summary>
    public string? AfterData => AfterJson;
    
    /// <summary>操作分组 ID（批量操作共享）</summary>
    public string? GroupId { get; set; }
    
    /// <summary>人类可读描述</summary>
    public string? Description { get; set; }
    
    /// <summary>会话 ID</summary>
    public string? SessionId { get; set; }
    
    /// <summary>创建时间</summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    /// <summary>是否已撤销</summary>
    public bool Undone { get; set; } = false;
}
