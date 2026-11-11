using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TKCompass.Domain.Services;

namespace TKCompass.ViewModels;

/// <summary>
/// 设置视图 ViewModel
/// </summary>
public partial class SettingsViewModel : ObservableObject
{
    private readonly IOperationLogService _operationLogService;

    [ObservableProperty]
    private string _databasePath = "";

    [ObservableProperty]
    private int _logRetentionDays = 30;

    [ObservableProperty]
    private int _minimumLogCount = 1000;

    [ObservableProperty]
    private bool _autoBackup = true;

    [ObservableProperty]
    private string _backupPath = "";

    [ObservableProperty]
    private string _statusMessage = "";

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private bool _isLoading;

    public SettingsViewModel(IOperationLogService operationLogService)
    {
        _operationLogService = operationLogService;
        LoadSettings();
    }

    private void LoadSettings()
    {
        try
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var settingsFile = Path.Combine(appDataPath, "TKCompass", "settings.json");

            if (File.Exists(settingsFile))
            {
                var json = File.ReadAllText(settingsFile);
                var settings = System.Text.Json.JsonSerializer.Deserialize<SettingsData>(json);

                if (settings != null)
                {
                    DatabasePath = settings.DatabasePath ?? Path.Combine(appDataPath, "TKCompass", "tkcompass.db");
                    BackupPath = settings.BackupPath ?? Path.Combine(appDataPath, "TKCompass", "backups");
                    LogRetentionDays = settings.LogRetentionDays;
                    MinimumLogCount = settings.MinimumLogCount;
                    AutoBackup = settings.AutoBackup;
                    return;
                }
            }
        }
        catch
        {
            // 加载失败时使用默认值
        }

        // 默认值
        var defaultAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        DatabasePath = Path.Combine(defaultAppDataPath, "TKCompass", "tkcompass.db");
        BackupPath = Path.Combine(defaultAppDataPath, "TKCompass", "backups");
        LogRetentionDays = 30;
        MinimumLogCount = 1000;
        AutoBackup = true;
    }

    // 辅助类用于反序列化
    private class SettingsData
    {
        public string? DatabasePath { get; set; }
        public string? BackupPath { get; set; }
        public int LogRetentionDays { get; set; }
        public int MinimumLogCount { get; set; }
        public bool AutoBackup { get; set; }
    }

    [RelayCommand]
    private async Task CleanOldLogsAsync()
    {
        IsLoading = true;
        try
        {
            StatusMessage = "正在清理旧日志...";
            HasError = false;

            await _operationLogService.PruneOldLogsAsync(LogRetentionDays, MinimumLogCount);

            StatusMessage = $"成功清理 {LogRetentionDays} 天前的日志（保留至少 {MinimumLogCount} 条）";
        }
        catch (Exception ex)
        {
            StatusMessage = $"清理失败: {ex.Message}";
            HasError = true;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void SaveSettings()
    {
        try
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var settingsDir = Path.Combine(appDataPath, "TKCompass");
            Directory.CreateDirectory(settingsDir);

            var settingsFile = Path.Combine(settingsDir, "settings.json");
            var settings = new
            {
                DatabasePath,
                BackupPath,
                LogRetentionDays,
                MinimumLogCount,
                AutoBackup
            };

            var json = System.Text.Json.JsonSerializer.Serialize(settings, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(settingsFile, json);

            StatusMessage = "设置已保存";
            HasError = false;
        }
        catch (Exception ex)
        {
            StatusMessage = $"保存失败: {ex.Message}";
            HasError = true;
        }
    }

    [RelayCommand]
    private async Task BrowseDatabasePath()
    {
        try
        {
            // 注意：Avalonia 的文件对话框需要通过 View 层调用
            // 这里使用 Messenger 通知 View 层打开对话框
            // 暂时使用简单实现：直接设置路径
            StatusMessage = "请在界面中手动输入数据库路径，或使用文件管理器复制路径";
        }
        catch (Exception ex)
        {
            StatusMessage = $"浏览失败: {ex.Message}";
            HasError = true;
        }
    }

    [RelayCommand]
    private async Task BrowseBackupPath()
    {
        try
        {
            StatusMessage = "请在界面中手动输入备份路径，或使用文件管理器复制路径";
        }
        catch (Exception ex)
        {
            StatusMessage = $"浏览失败: {ex.Message}";
            HasError = true;
        }
    }
}
