using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TKCompass.Domain.Models;
using TKCompass.Domain.Services;

namespace TKCompass.ViewModels;

/// <summary>
/// 达人工作台 ViewModel（6 Tab）
/// </summary>
public partial class CreatorDashboardViewModel : ObservableObject, IRecipient<CreatorUpdatedMessage>
{
    private readonly ICreatorService _creatorService;
    private readonly IMessenger _messenger;

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

    public CreatorDashboardViewModel(ICreatorService creatorService, IMessenger messenger)
    {
        _creatorService = creatorService;
        _messenger = messenger;
        _messenger.Register(this);
        _ = LoadDataAsync();
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

        if (!string.IsNullOrEmpty(Filter.ProductNameSearch))
        {
            // TODO: 需要关联产品名搜索
        }

        FilteredCreators = new ObservableCollection<Creator>(filtered);
    }

    [RelayCommand]
    private void ClearFilter()
    {
        Filter.Reset();
        FilteredCreators = new ObservableCollection<Creator>(AllCreators);
    }
}

/// <summary>
/// 达人更新消息（用于 Messenger 通信）
/// </summary>
public record CreatorUpdatedMessage;
