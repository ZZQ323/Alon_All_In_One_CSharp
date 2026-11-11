namespace TKCompass.Domain.Services;

/// <summary>
/// 备份服务实现
/// </summary>
public class BackupService : IBackupService
{
    private const string BackupPrefix = "tkcompass.db.bak.";
    private const int MaxBackups = 48; // 保留 24 小时（每半小时备份一次）
    
    private readonly string _dbPath;
    private readonly string _backupDir;

    public BackupService(string dbPath, string backupDir)
    {
        _dbPath = dbPath;
        _backupDir = backupDir;
    }

    public async Task<string> CreateBackupAsync()
    {
        if (!File.Exists(_dbPath))
            throw new FileNotFoundException($"数据库文件不存在: {_dbPath}");

        EnsureBackupDirectory();

        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var backupFileName = BackupPrefix + timestamp;
        var backupPath = Path.Combine(_backupDir, backupFileName);

        await Task.Run(() => File.Copy(_dbPath, backupPath, true));

        await RotateBackupsAsync();

        return backupPath;
    }

    public async Task<string?> AutoBackupIfNeededAsync()
    {
        if (await HasBackupTodayAsync())
            return null;

        return await CreateBackupAsync();
    }

    public async Task RestoreBackupAsync(string backupFilePath)
    {
        if (!File.Exists(backupFilePath))
            throw new FileNotFoundException($"备份文件不存在: {backupFilePath}");

        await Task.Run(() => File.Copy(backupFilePath, _dbPath, true));
    }

    public async Task<IEnumerable<BackupFileInfo>> GetBackupListAsync()
    {
        return await Task.Run(() =>
        {
            if (!Directory.Exists(_backupDir))
                return Enumerable.Empty<BackupFileInfo>();

            var files = Directory.GetFiles(_backupDir, $"{BackupPrefix}*")
                .Select(f => new FileInfo(f))
                .OrderByDescending(f => f.CreationTime)
                .Select(f => new BackupFileInfo
                {
                    FileName = f.Name,
                    FullPath = f.FullName,
                    CreatedAt = f.CreationTime,
                    FileSize = f.Length
                });

            return files;
        });
    }

    public async Task DeleteBackupAsync(string backupFilePath)
    {
        await Task.Run(() =>
        {
            if (File.Exists(backupFilePath))
                File.Delete(backupFilePath);
        });
    }

    private async Task<bool> HasBackupTodayAsync()
    {
        return await Task.Run(() =>
        {
            if (!Directory.Exists(_backupDir))
                return false;

            var today = DateTime.Now.ToString("yyyyMMdd");
            var files = Directory.GetFiles(_backupDir, $"{BackupPrefix}*{today}*");
            return files.Length > 0;
        });
    }

    private void EnsureBackupDirectory()
    {
        if (!Directory.Exists(_backupDir))
            Directory.CreateDirectory(_backupDir);
    }

    private async Task RotateBackupsAsync()
    {
        var backups = (await GetBackupListAsync()).ToList();
        if (backups.Count <= MaxBackups)
            return;

        var toDelete = backups.Skip(MaxBackups);
        foreach (var backup in toDelete)
        {
            await DeleteBackupAsync(backup.FullPath);
        }
    }
}
