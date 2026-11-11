namespace TKCompass.Domain.Services;

/// <summary>
/// 备份服务接口
/// </summary>
public interface IBackupService
{
    /// <summary>
    /// 创建手动备份
    /// </summary>
    /// <returns>备份文件路径</returns>
    Task<string> CreateBackupAsync();
    
    /// <summary>
    /// 自动备份（如果今天还没有备份）
    /// </summary>
    /// <returns>备份文件路径，如果已存在则返回 null</returns>
    Task<string?> AutoBackupIfNeededAsync();
    
    /// <summary>
    /// 恢复备份
    /// </summary>
    Task RestoreBackupAsync(string backupFilePath);
    
    /// <summary>
    /// 获取备份列表
    /// </summary>
    Task<IEnumerable<BackupFileInfo>> GetBackupListAsync();
    
    /// <summary>
    /// 删除备份文件
    /// </summary>
    Task DeleteBackupAsync(string backupFilePath);
}

/// <summary>
/// 备份文件信息
/// </summary>
public class BackupFileInfo
{
    public string FileName { get; set; } = string.Empty;
    public string FullPath { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public long FileSize { get; set; }
}
