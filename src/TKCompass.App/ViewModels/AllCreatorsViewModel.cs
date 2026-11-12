using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TKCompass.Domain.Models;
using TKCompass.Domain.Services;
using TKCompass.Services;

namespace TKCompass.ViewModels;

/// <summary>
/// 达人总表独立页面 ViewModel
/// </summary>
public partial class AllCreatorsViewModel : ObservableObject, IRecipient<CreatorUpdatedMessage>
{
    private readonly ICreatorService _creatorService;
    private readonly IMessenger _messenger;
    private readonly INotificationService? _notificationService;

    [ObservableProperty]
    private ObservableCollection<Creator> _allCreators = new();

    [ObservableProperty]
    private ObservableCollection<Creator> _filteredCreators = new();

    [ObservableProperty]
    private string _selectedOverviewStatus = "全部";

    [ObservableProperty]
    private ObservableCollection<Creator> _selectedCreators = new();

    [ObservableProperty]
    private bool _hasSelection;

    [ObservableProperty]
    private bool _isLoading;

    public AllCreatorsViewModel(
        ICreatorService creatorService,
        IMessenger messenger,
        INotificationService? notificationService = null)
    {
        _creatorService = creatorService;
        _messenger = messenger;
        _notificationService = notificationService;
        _messenger.Register(this);

        SelectedCreators.CollectionChanged += (s, e) => HasSelection = SelectedCreators.Count > 0;

        _ = LoadDataAsync();
    }

    partial void OnSelectedCreatorsChanged(ObservableCollection<Creator> value)
    {
        if (value != null)
        {
            value.CollectionChanged += (s, e) => HasSelection = value.Count > 0;
        }
        HasSelection = value?.Count > 0;
    }

    partial void OnSelectedOverviewStatusChanged(string value)
    {
        ApplyOverviewStatusFilter();
    }

    private async Task LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            var creators = await _creatorService.GetAllIncludeDeletedAsync();
            var list = creators.ToList();

            AllCreators = new ObservableCollection<Creator>(list.Where(c => !c.Deleted));
            FilteredCreators = new ObservableCollection<Creator>(AllCreators);
        }
        catch (Exception ex)
        {
            if (_notificationService != null)
                await _notificationService.ShowErrorAsync("加载数据失败", ex.Message);
            else
                System.Diagnostics.Debug.WriteLine($"加载数据失败: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    public void Receive(CreatorUpdatedMessage message)
    {
        _ = LoadDataAsync();
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        await LoadDataAsync();
    }

    [RelayCommand]
    private void ApplyFilter()
    {
        ApplyOverviewStatusFilter();
    }

    [RelayCommand]
    private void ClearFilter()
    {
        SelectedOverviewStatus = "全部";
        FilteredCreators = new ObservableCollection<Creator>(AllCreators);
    }

    [RelayCommand]
    private async Task BatchDeleteAsync()
    {
        if (SelectedCreators == null || SelectedCreators.Count == 0)
            return;

        if (_notificationService != null)
        {
            var confirmed = await _notificationService.ConfirmAsync(
                "确认删除",
                $"确定要删除选中的 {SelectedCreators.Count} 个达人吗？此操作可以撤销。");
            if (!confirmed)
                return;
        }

        try
        {
            foreach (var creator in SelectedCreators.ToList())
            {
                await _creatorService.DeleteAsync(creator.Id);
            }

            SelectedCreators.Clear();
            _messenger.Send(new CreatorUpdatedMessage());

            if (_notificationService != null)
                await _notificationService.ShowInfoAsync("成功", "批量删除完成");
        }
        catch (Exception ex)
        {
            if (_notificationService != null)
                await _notificationService.ShowErrorAsync("删除失败", ex.Message);
        }
    }

    [RelayCommand]
    private async Task BatchRestoreAsync()
    {
        if (SelectedCreators == null || SelectedCreators.Count == 0)
            return;

        try
        {
            foreach (var creator in SelectedCreators.ToList())
            {
                await _creatorService.RestoreAsync(creator.Id);
            }

            SelectedCreators.Clear();
            _messenger.Send(new CreatorUpdatedMessage());

            if (_notificationService != null)
                await _notificationService.ShowInfoAsync("成功", "批量恢复完成");
        }
        catch (Exception ex)
        {
            if (_notificationService != null)
                await _notificationService.ShowErrorAsync("恢复失败", ex.Message);
        }
    }

    [RelayCommand]
    private async Task DeletePermanentlyAsync(Creator creator)
    {
        if (creator == null)
            return;

        if (_notificationService != null)
        {
            var confirmed = await _notificationService.ConfirmAsync(
                "确认永久删除",
                $"确定要永久删除达人 {creator.Nickname} 吗？此操作不可撤销！");
            if (!confirmed)
                return;
        }

        try
        {
            await _creatorService.PermanentDeleteAsync(creator.Id);
            _messenger.Send(new CreatorUpdatedMessage());

            if (_notificationService != null)
                await _notificationService.ShowInfoAsync("成功", $"已永久删除 {creator.Nickname}");
        }
        catch (Exception ex)
        {
            if (_notificationService != null)
                await _notificationService.ShowErrorAsync("删除失败", ex.Message);
        }
    }

    [RelayCommand]
    private void OpenCreatorDetail(Creator creator)
    {
        if (creator == null)
            return;

        _messenger.Send(new OpenCreatorDetailMessage(creator.Id));
    }

    [RelayCommand]
    private async Task CreateCreatorAsync()
    {
        // 发送消息让主窗口打开新建达人弹窗
        _messenger.Send(new OpenCreateCreatorDialogMessage());
    }

    private void ApplyOverviewStatusFilter()
    {
        if (AllCreators == null || AllCreators.Count == 0)
            return;

        var filtered = AllCreators.AsEnumerable();

        if (!string.IsNullOrEmpty(SelectedOverviewStatus) && SelectedOverviewStatus != "全部")
        {
            filtered = filtered.Where(c => c.CooperationStatus.ToString() == SelectedOverviewStatus);
        }

        FilteredCreators = new ObservableCollection<Creator>(filtered);
    }
}
