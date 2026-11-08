using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TKCompass.Domain.Models;
using TKCompass.Domain.Services;

namespace TKCompass.ViewModels;

/// <summary>
/// 操作历史视图 ViewModel
/// </summary>
public partial class OperationHistoryViewModel : ObservableObject
{
    private readonly IOperationLogService _operationLogService;
    private readonly IUndoService _undoService;
    private readonly IMessenger _messenger;

    [ObservableProperty]
    private ObservableCollection<OperationLog> _operationLogs = new();

    [ObservableProperty]
    private OperationLog? _selectedLog;

    [ObservableProperty]
    private string _statusMessage = "";

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private bool _isLoading;

    public OperationHistoryViewModel(
        IOperationLogService operationLogService,
        IUndoService undoService,
        IMessenger messenger)
    {
        _operationLogService = operationLogService;
        _undoService = undoService;
        _messenger = messenger;
        _ = LoadLogsAsync();
    }

    [RelayCommand]
    private async Task LoadLogsAsync()
    {
        try
        {
            IsLoading = true;
            HasError = false;
            StatusMessage = "";

            var logs = await _operationLogService.GetRecentAsync(1000);
            OperationLogs = new ObservableCollection<OperationLog>(logs);
        }
        catch (Exception ex)
        {
            StatusMessage = $"加载失败: {ex.Message}";
            HasError = true;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        await LoadLogsAsync();
    }

    [RelayCommand]
    private async Task UndoAsync()
    {
        if (SelectedLog == null)
        {
            StatusMessage = "请先选择要撤销的操作";
            HasError = true;
            return;
        }

        try
        {
            IsLoading = true;
            HasError = false;
            StatusMessage = "";

            bool success = await _undoService.UndoLastAsync();
            
            if (success)
            {
                StatusMessage = "撤销成功";
                _messenger.Send(new CreatorUpdatedMessage());
                await LoadLogsAsync();
            }
            else
            {
                StatusMessage = "撤销失败：无法撤销此操作";
                HasError = true;
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"撤销失败: {ex.Message}";
            HasError = true;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task UndoGroupAsync()
    {
        if (SelectedLog == null || string.IsNullOrEmpty(SelectedLog.GroupId))
        {
            StatusMessage = "请先选择要撤销的操作组";
            HasError = true;
            return;
        }

        try
        {
            IsLoading = true;
            HasError = false;
            StatusMessage = "";

            bool success = await _undoService.UndoGroupAsync(SelectedLog.GroupId);
            
            if (success)
            {
                StatusMessage = "撤销操作组成功";
                _messenger.Send(new CreatorUpdatedMessage());
                await LoadLogsAsync();
            }
            else
            {
                StatusMessage = "撤销失败：无法撤销此操作组";
                HasError = true;
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"撤销失败: {ex.Message}";
            HasError = true;
        }
        finally
        {
            IsLoading = false;
        }
    }
}
