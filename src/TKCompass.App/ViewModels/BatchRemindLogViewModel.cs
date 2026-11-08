using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TKCompass.Domain.Models;
using TKCompass.Domain.Services;

namespace TKCompass.ViewModels;

public partial class BatchRemindLogViewModel : ObservableObject
{
    private readonly IBatchRemindLogService _batchRemindLogService;

    [ObservableProperty]
    private ObservableCollection<BatchRemindLog> _logs = new();

    [ObservableProperty]
    private BatchRemindLog? _selectedLog;

    [ObservableProperty]
    private bool _isLoading;

    public BatchRemindLogViewModel(IBatchRemindLogService batchRemindLogService)
    {
        _batchRemindLogService = batchRemindLogService;
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            var logs = await _batchRemindLogService.GetAllAsync();
            Logs = new ObservableCollection<BatchRemindLog>(logs);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        await LoadDataAsync();
    }
}
