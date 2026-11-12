using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TKCompass.Domain.Enums;
using TKCompass.Domain.Models;
using TKCompass.Domain.Services;
using TKCompass.Services;

namespace TKCompass.ViewModels;

/// <summary>
/// 达人工作台 ViewModel（2 Tab：今日提醒、正在交付）
/// </summary>
public partial class CreatorDashboardViewModel : ObservableObject, IRecipient<CreatorUpdatedMessage>
{
    private readonly ICreatorService _creatorService;
    private readonly IMessenger _messenger;
    private readonly INotificationService? _notificationService;
    private readonly IOperationLogService? _operationLogService;

    [ObservableProperty]
    private ObservableCollection<Creator> _allCreators = new();

    [ObservableProperty]
    private ObservableCollection<Creator> _filteredCreators = new();

    [ObservableProperty]
    private ObservableCollection<Creator> _todayReminderCreators = new();

    [ObservableProperty]
    private ObservableCollection<Creator> _inDeliveryCreators = new();

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
    private ObservableCollection<Creator> _selectedDeliveryCreators = new();

    [ObservableProperty]
    private bool _hasDeliverySelection;

    public CreatorDashboardViewModel(
        ICreatorService creatorService,
        IMessenger messenger,
        INotificationService? notificationService = null,
        IOperationLogService? operationLogService = null)
    {
        _creatorService = creatorService;
        _messenger = messenger;
        _notificationService = notificationService;
        _operationLogService = operationLogService;
        _messenger.Register(this);

        SelectedDeliveryCreators.CollectionChanged += (s, e) => HasDeliverySelection = SelectedDeliveryCreators.Count > 0;

        // 注入 Filter 回调
        Filter.OnRefresh = () => _ = RefreshAsync();
        Filter.OnApplyFilter = ApplyFilter;
        Filter.OnClearFilter = ClearFilter;
        Filter.OnCreateCreator = CreateCreator;

        _ = LoadDataAsync();
    }

    partial void OnSelectedDeliveryCreatorsChanged(ObservableCollection<Creator> value)
    {
        if (value != null)
        {
            value.CollectionChanged += (s, e) => HasDeliverySelection = value.Count > 0;
        }
        HasDeliverySelection = value?.Count > 0;
    }

    private async Task LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            var creators = await _creatorService.GetAllIncludeDeletedAsync();
            var list = creators.ToList();

            // 所有未删除的达人（用于筛选）
            AllCreators = new ObservableCollection<Creator>(list.Where(c => !c.Deleted));
            FilteredCreators = new ObservableCollection<Creator>(AllCreators);
            
            // 今日提醒：今日状态为待跟进的未删除达人
            TodayReminderCreators = new ObservableCollection<Creator>(list.Where(c => c.TodayStatus == TodayStatus.Pending && !c.Deleted));
            
            // 正在交付：同意合作或寄样中，且未交付的未删除达人
            InDeliveryCreators = new ObservableCollection<Creator>(list.Where(c => 
                (c.CooperationStatus == CooperationStatus.CooperationAgreed || 
                 c.CooperationStatus == CooperationStatus.SampleSent) && 
                !c.Delivered && !c.Deleted));

            // 更新统计数字
            UpdateStatistics(list);
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

    /// <summary>
    /// 更新统计数字（顶部统计栏）
    /// </summary>
    private void UpdateStatistics(List<Creator> allCreators)
    {
        TotalCount = AllCreators.Count;
        DeliveredCount = allCreators.Count(c => c.Delivered && !c.Deleted);
        DeletedCount = allCreators.Count(c => c.Deleted);
        RemindedCount = allCreators.Count(c => c.TodayStatus == TodayStatus.Reminded && !c.Deleted);
        DeliveringCount = InDeliveryCreators.Count;
        TodayFollowUpCount = TodayReminderCreators.Count;
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
    private void CreateCreator()
    {
        // 通过 Messenger 通知 View 层打开弹窗（解耦 DI Scoped 生命周期）
        _messenger.Send(new OpenCreateCreatorDialogMessage());
    }

    [RelayCommand]
    private void ApplyFilter()
    {
        var filtered = AllCreators.AsEnumerable();

        // 应用通用筛选条件
        filtered = ApplyCommonFilter(filtered);

        FilteredCreators = new ObservableCollection<Creator>(filtered);

        // 今日提醒 Tab：在通用筛选基础上，叠加"今日状态为待跟进"
        var todayReminder = AllCreators.AsEnumerable()
            .Where(c => c.TodayStatus == TodayStatus.Pending);
        todayReminder = ApplyCommonFilter(todayReminder);
        TodayReminderCreators = new ObservableCollection<Creator>(todayReminder);

        // 正在交付 Tab：在通用筛选基础上，叠加"同意合作或寄样中，且未交付"
        var inDelivery = AllCreators.AsEnumerable()
            .Where(c => (c.CooperationStatus == CooperationStatus.CooperationAgreed || 
                         c.CooperationStatus == CooperationStatus.SampleSent) && 
                        !c.Delivered);
        inDelivery = ApplyCommonFilter(inDelivery);
        InDeliveryCreators = new ObservableCollection<Creator>(inDelivery);

        // 筛选后更新统计数字
        UpdateStatistics(AllCreators.ToList());
    }

    /// <summary>
    /// 应用通用筛选条件（提取公共逻辑）
    /// </summary>
    private IEnumerable<Creator> ApplyCommonFilter(IEnumerable<Creator> source)
    {
        var filtered = source;

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

        return filtered;
    }

    [RelayCommand]
    private void ClearFilter()
    {
        Filter.Reset();
        FilteredCreators = new ObservableCollection<Creator>(AllCreators);
        
        // 重置今日提醒 Tab
        TodayReminderCreators = new ObservableCollection<Creator>(
            AllCreators.Where(c => c.TodayStatus == TodayStatus.Pending));
        
        // 重置正在交付 Tab
        InDeliveryCreators = new ObservableCollection<Creator>(
            AllCreators.Where(c => 
                (c.CooperationStatus == CooperationStatus.CooperationAgreed || 
                 c.CooperationStatus == CooperationStatus.SampleSent) && 
                !c.Delivered));

        // 清除筛选后更新统计数字
        UpdateStatistics(AllCreators.ToList());
    }

    [RelayCommand]
    private void OpenCreatorDetail(Creator creator)
    {
        if (creator == null)
            return;

        _messenger.Send(new OpenCreatorDetailMessage(creator.Id));
    }

    #region Tab1 - 今日提醒功能

    /// <summary>
    /// 推进状态 - 将今日状态从待跟进推进到已提醒
    /// </summary>
    [RelayCommand]
    private async Task AdvanceStatusAsync(Creator creator)
    {
        if (creator == null)
            return;

        try
        {
            // 校验必填项：确保有昵称和达人ID
            if (string.IsNullOrWhiteSpace(creator.Nickname) || string.IsNullOrWhiteSpace(creator.Handle))
            {
                if (_notificationService != null)
                    await _notificationService.ShowErrorAsync("校验失败", "达人必须有昵称和达人ID才能推进状态");
                return;
            }

            // 推进状态：Pending -> Reminded
            await _creatorService.UpdateTodayStatusAsync(creator.Id, TodayStatus.Reminded);
            
            if (_notificationService != null)
                await _notificationService.ShowInfoAsync("成功", $"已将 {creator.Nickname} 的状态推进为已提醒");
        }
        catch (Exception ex)
        {
            if (_notificationService != null)
                await _notificationService.ShowErrorAsync("推进状态失败", ex.Message);
        }
    }

    /// <summary>
    /// 标记已提醒
    /// </summary>
    [RelayCommand]
    private async Task MarkRemindedAsync(Creator creator)
    {
        if (creator == null)
            return;

        try
        {
            await _creatorService.UpdateTodayStatusAsync(creator.Id, TodayStatus.Reminded);
            
            if (_notificationService != null)
                await _notificationService.ShowInfoAsync("成功", $"已标记 {creator.Nickname} 为已提醒");
        }
        catch (Exception ex)
        {
            if (_notificationService != null)
                await _notificationService.ShowErrorAsync("标记失败", ex.Message);
        }
    }

    /// <summary>
    /// 标记暂不合作 - 将合作状态设置为已解决
    /// </summary>
    [RelayCommand]
    private async Task MarkNotCooperatingAsync(Creator creator)
    {
        if (creator == null)
            return;

        try
        {
            await _creatorService.UpdateTodayStatusAsync(creator.Id, TodayStatus.Resolved);
            
            if (_notificationService != null)
                await _notificationService.ShowInfoAsync("成功", $"已标记 {creator.Nickname} 为暂不合作");
        }
        catch (Exception ex)
        {
            if (_notificationService != null)
                await _notificationService.ShowErrorAsync("标记失败", ex.Message);
        }
    }

    #endregion

    #region Tab2 - 正在交付功能

    /// <summary>
    /// 批量提醒 - 对选中的正在交付的达人进行提醒，并写入操作日志
    /// </summary>
    [RelayCommand]
    private async Task BatchRemindAsync()
    {
        if (SelectedDeliveryCreators == null || SelectedDeliveryCreators.Count == 0)
        {
            if (_notificationService != null)
                await _notificationService.ShowErrorAsync("提示", "请先选择要提醒的达人");
            return;
        }

        try
        {
            var groupId = Guid.NewGuid().ToString();
            var count = 0;

            foreach (var creator in SelectedDeliveryCreators.ToList())
            {
                // 更新今日状态为已提醒
                await _creatorService.UpdateTodayStatusAsync(creator.Id, TodayStatus.Reminded);

                // 写入操作日志
                if (_operationLogService != null)
                {
                    var beforeJson = JsonSerializer.Serialize(new { TodayStatus = TodayStatus.Pending });
                    var afterJson = JsonSerializer.Serialize(new { TodayStatus = TodayStatus.Reminded });
                    await _operationLogService.LogOperationAsync(
                        operationType: "Update",
                        entityType: "Creator",
                        entityId: creator.Id.ToString(),
                        beforeJson: beforeJson,
                        afterJson: afterJson,
                        groupId: groupId,
                        description: $"批量提醒: {creator.Nickname}");
                }

                count++;
            }

            SelectedDeliveryCreators.Clear();
            
            if (_notificationService != null)
                await _notificationService.ShowInfoAsync("成功", $"已提醒 {count} 个达人");
        }
        catch (Exception ex)
        {
            if (_notificationService != null)
                await _notificationService.ShowErrorAsync("批量提醒失败", ex.Message);
        }
    }

    #endregion

}

/// <summary>
/// 打开达人详情消息（用于导航到详情页）
/// </summary>
public record OpenCreatorDetailMessage(long CreatorId);

/// <summary>
/// 达人更新消息（用于 Messenger 通信）
/// </summary>
public record CreatorUpdatedMessage;

/// <summary>
/// 打开新建达人弹窗消息
/// </summary>
public record OpenCreateCreatorDialogMessage;
