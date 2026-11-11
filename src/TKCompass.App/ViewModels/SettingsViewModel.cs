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
        // 从配置文件或注册表加载设置
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        DatabasePath = Path.Combine(appDataPath, "TKCompass", "tkcompass.db");
        BackupPath = Path.Combine(appDataPath, "TKCompass", "backups");
        LogRetentionDays = 30;
        MinimumLogCount = 1000;
        AutoBackup = true;
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
            // TODO: 保存到配置文件或注册表
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
    private void BrowseDatabasePath()
    {
        // TODO: 打开文件选择对话框
    }

    [RelayCommand]
    private void BrowseBackupPath()
    {
        // TODO: 打开文件夹选择对话框
    }
}
