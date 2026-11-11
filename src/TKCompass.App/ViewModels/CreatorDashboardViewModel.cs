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
/// 达人工作台 ViewModel（6 Tab）
/// </summary>
public partial class CreatorDashboardViewModel : ObservableObject, IRecipient<CreatorUpdatedMessage>
{
    private readonly ICreatorService _creatorService;
    private readonly IMessenger _messenger;
    private readonly INotificationService? _notificationService;

    [ObservableProperty]
    private ObservableCollection<Creator> _allCreators = new();

    [ObservableProperty]
    private ObservableCollection<Creator> _filteredCreators = new();

    [ObservableProperty]
    private ObservableCollection<Creator> _deliveredCreators = new();

    [ObservableProperty]
    private ObservableCollection<Creator> _deletedCreators = new();

    [ObservableProperty]
    private ObservableCollection<Creator> _remindedCreators = new();

    [ObservableProperty]
    private ObservableCollection<Creator> _deliveringCreators = new();

    [ObservableProperty]
    private ObservableCollection<Creator> _todayFollowUpCreators = new();

    [ObservableProperty]
    private FilterViewModel _filter = new();

    [ObservableProperty]
    private int _selectedTabIndex;

    [ObservableProperty]
    private int _totalCount;

    [ObservableProperty]
    private int _deliveredCount;

    [ObservableProperty]
    private int _deletedCount;

    [ObservableProperty]
    private int _remindedCount;

    [ObservableProperty]
    private int _deliveringCount;

    [ObservableProperty]
    private int _todayFollowUpCount;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private ObservableCollection<Creator> _selectedCreators = new();

    [ObservableProperty]
    private bool _hasSelection;

    public CreatorDashboardViewModel(ICreatorService creatorService, IMessenger messenger, INotificationService? notificationService = null)
    {
        _creatorService = creatorService;
        _messenger = messenger;
        _notificationService = notificationService;
        _messenger.Register(this);

        // Subscribe to SelectedCreators.CollectionChanged so HasSelection stays in sync
        // when items are added/removed from the existing collection (e.g. by the code-behind).
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

    private async Task LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            var creators = await _creatorService.GetAllIncludeDeletedAsync();
            var list = creators.ToList();

            AllCreators = new ObservableCollection<Creator>(list.Where(c => !c.Deleted));
            FilteredCreators = new ObservableCollection<Creator>(AllCreators);
            
            DeliveredCreators = new ObservableCollection<Creator>(list.Where(c => c.Delivered && !c.Deleted));
            DeletedCreators = new ObservableCollection<Creator>(list.Where(c => c.Deleted));
            RemindedCreators = new ObservableCollection<Creator>(list.Where(c => c.TodayStatus == Domain.Enums.TodayStatus.Reminded && !c.Deleted));
            DeliveringCreators = new ObservableCollection<Creator>(list.Where(c => c.CooperationStatus == Domain.Enums.CooperationStatus.SampleSent && !c.Delivered && !c.Deleted));
            TodayFollowUpCreators = new ObservableCollection<Creator>(list.Where(c => c.TodayStatus == Domain.Enums.TodayStatus.Pending && !c.Deleted));

            TotalCount = AllCreators.Count;
            DeliveredCount = DeliveredCreators.Count;
            DeletedCount = DeletedCreators.Count;
            RemindedCount = RemindedCreators.Count;
            DeliveringCount = DeliveringCreators.Count;
            TodayFollowUpCount = TodayFollowUpCreators.Count;
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
        var filtered = AllCreators.AsEnumerable();

        if (!string.IsNullOrEmpty(Filter.HandleSearch))
        {
            filtered = filtered.Where(c => c.Handle.Contains(Filter.HandleSearch, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(Filter.SelectedStatus))
        {
            filtered = filtered.Where(c => c.CooperationStatus.ToString() == Filter.SelectedStatus);
        }

        if (!string.IsNullOrEmpty(Filter.SelectedQualityLevel))
        {
            filtered = filtered.Where(c => c.QualityLevel == Filter.SelectedQualityLevel);
        }

        if (Filter.SampleDateFrom.HasValue)
        {
            filtered = filtered.Where(c => c.FirstSampleDate >= Filter.SampleDateFrom.Value);
        }

        if (Filter.SampleDateTo.HasValue)
        {
            filtered = filtered.Where(c => c.FirstSampleDate <= Filter.SampleDateTo.Value);
        }

        if (Filter.ContactDateFrom.HasValue)
        {
            filtered = filtered.Where(c => c.LatestContactDate >= Filter.ContactDateFrom.Value);
        }

        if (Filter.ContactDateTo.HasValue)
        {
            filtered = filtered.Where(c => c.LatestContactDate <= Filter.ContactDateTo.Value);
        }

        // P1 任务 8：产品代码多选筛选
        if (Filter.SelectedProductCodes != null && Filter.SelectedProductCodes.Count > 0)
        {
            filtered = filtered.Where(c => 
                c.Shipments != null && 
                c.Shipments.Any(s => 
                    !string.IsNullOrEmpty(s.Sku) &&
                    Filter.SelectedProductCodes.Any(code => 
                        s.Sku.Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(x => x.Trim())
                            .Contains(code, StringComparer.OrdinalIgnoreCase))));
        }

        // P1 任务 9：产品名搜索筛选
        if (!string.IsNullOrEmpty(Filter.ProductNameSearch))
        {
            var search = Filter.ProductNameSearch.Trim();
            filtered = filtered.Where(c => 
                c.Shipments != null && 
                c.Shipments.Any(s => 
                    !string.IsNullOrEmpty(s.ProductName) &&
                    s.ProductName.Contains(search, StringComparison.OrdinalIgnoreCase)));
        }

        FilteredCreators = new ObservableCollection<Creator>(filtered);
    }

    [RelayCommand]
    private void ClearFilter()
    {
        Filter.Reset();
        FilteredCreators = new ObservableCollection<Creator>(AllCreators);
    }

    [RelayCommand]
    private async Task BatchDeleteAsync()
    {
        if (SelectedCreators == null || SelectedCreators.Count == 0)
            return;

        // 使用通知服务确认批量删除
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
    private void OpenCreatorDetail(Creator creator)
    {
        if (creator == null)
            return;

        _messenger.Send(new OpenCreatorDetailMessage(creator.Id));
    }
}

/// <summary>
/// 打开达人详情消息（用于导航到详情页）
/// </summary>
public record OpenCreatorDetailMessage(long CreatorId);

/// <summary>
/// 达人更新消息（用于 Messenger 通信）
/// </summary>
public record CreatorUpdatedMessage;
