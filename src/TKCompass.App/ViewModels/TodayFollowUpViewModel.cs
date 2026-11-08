using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TKCompass.Domain.Models;
using TKCompass.Domain.Services;
using System.Collections.ObjectModel;

namespace TKCompass.ViewModels;

public partial class TodayFollowUpViewModel : ObservableObject, IRecipient<CreatorUpdatedMessage>
{
    private readonly ICreatorService _creatorService;
    private readonly IShipmentService _shipmentService;
    private readonly IVideoService _videoService;
    private readonly IRelationshipService _relationshipService;
    private readonly ICreatorContactService _contactService;
    private readonly IProductCodeService _productCodeService;
    private readonly IDictionaryService _dictionaryService;
    private readonly IMessenger _messenger;

    [ObservableProperty]
    private DateOnly _fromDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-30));

    [ObservableProperty]
    private DateOnly _toDate = DateOnly.FromDateTime(DateTime.Now);

    [ObservableProperty]
    private int _totalCount;

    [ObservableProperty]
    private int _pendingCount;

    [ObservableProperty]
    private int _acknowledgedCount;

    [ObservableProperty]
    private ObservableCollection<FollowUpGroup> _followUpGroups = new();

    [ObservableProperty]
    private int _currentPage = 1;

    [ObservableProperty]
    private int _totalPages = 1;

    [ObservableProperty]
    private string _pageInfo = "";

    private const int PageSize = 50;
    private List<FollowUpItem> _allItems = new();

    public TodayFollowUpViewModel(
        ICreatorService creatorService,
        IShipmentService shipmentService,
        IVideoService videoService,
        IRelationshipService relationshipService,
        ICreatorContactService contactService,
        IProductCodeService productCodeService,
        IDictionaryService dictionaryService,
        IMessenger messenger)
    {
        _creatorService = creatorService;
        _shipmentService = shipmentService;
        _videoService = videoService;
        _relationshipService = relationshipService;
        _contactService = contactService;
        _productCodeService = productCodeService;
        _dictionaryService = dictionaryService;
        _messenger = messenger;
        _messenger.Register(this);

        _ = LoadDataAsync();
    }

    public void Receive(CreatorUpdatedMessage message)
    {
        _ = LoadDataAsync();
    }

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        try
        {
            // 获取所有达人
            var creators = await _creatorService.GetAllAsync();
            var creatorList = creators.ToList();

            // 获取所有寄样记录
            var shipments = await _shipmentService.GetAllAsync();
            var shipmentList = shipments.ToList();

            // 获取所有视频
            var videos = await _videoService.GetAllAsync();
            var videoList = videos.ToList();

            // 获取所有关系
            var relationships = await _relationshipService.GetAllAsync();
            var relationshipList = relationships.ToList();

            // 获取所有联系方式
            var contacts = await _contactService.GetAllAsync();
            var contactList = contacts.ToList();

            // 获取所有产品代码
            var productCodes = await _productCodeService.GetAllAsync();
            var productCodeList = productCodes.ToList();

            // 构建跟进项
            _allItems = new List<FollowUpItem>();

            foreach (var creator in creatorList)
            {
                var creatorShipments = shipmentList.Where(s => s.CreatorId == creator.Id).ToList();
                var creatorVideos = videoList.Where(v => v.CreatorId == creator.Id).ToList();
                var creatorRelationship = relationshipList.FirstOrDefault(r => r.CreatorId == creator.Id);
                var creatorContacts = contactList.Where(c => c.CreatorId == creator.Id).ToList();

                // 检查是否需要跟进
                bool needsFollowUp = false;
                string followUpReason = "";

                // 检查寄样状态
                foreach (var shipment in creatorShipments)
                {
                    if (shipment.SampleDate >= FromDate && shipment.SampleDate <= ToDate)
                    {
                        // 检查是否有视频
                        var shipmentVideos = creatorVideos.Where(v => v.ShipmentId == shipment.Id).ToList();
                        if (shipmentVideos.Count == 0)
                        {
                            needsFollowUp = true;
                            followUpReason = $"寄样 {shipment.TrackingNumber} 未收到视频";
                            break;
                        }
                    }
                }

                // 检查合作状态
                if (creator.CooperationStatus == Domain.Enums.CooperationStatus.Reached &&
                    creator.LatestContactDate.HasValue &&
                    creator.LatestContactDate.Value < DateOnly.FromDateTime(DateTime.Now.AddDays(-7)))
                {
                    needsFollowUp = true;
                    followUpReason = "超过7天未联系";
                }

                if (needsFollowUp)
                {
                    _allItems.Add(new FollowUpItem
                    {
                        Creator = creator,
                        Shipments = creatorShipments,
                        Videos = creatorVideos,
                        Relationship = creatorRelationship,
                        Contacts = creatorContacts,
                        FollowUpReason = followUpReason,
                        Status = FollowUpStatus.Pending
                    });
                }
            }

            // 更新统计
            TotalCount = _allItems.Count;
            PendingCount = _allItems.Count(i => i.Status == FollowUpStatus.Pending);
            AcknowledgedCount = _allItems.Count(i => i.Status == FollowUpStatus.Acknowledged);

            // 分页
            TotalPages = (int)Math.Ceiling((double)_allItems.Count / PageSize);
            CurrentPage = 1;
            UpdatePageInfo();
            UpdateFollowUpGroups();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"加载今日跟进数据失败: {ex.Message}");
        }
    }

    [RelayCommand]
    private void Search()
    {
        _ = LoadDataAsync();
    }

    [RelayCommand]
    private void Reset()
    {
        FromDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-30));
        ToDate = DateOnly.FromDateTime(DateTime.Now);
        _ = LoadDataAsync();
    }

    [RelayCommand]
    private void PreviousPage()
    {
        if (CurrentPage > 1)
        {
            CurrentPage--;
            UpdatePageInfo();
            UpdateFollowUpGroups();
        }
    }

    [RelayCommand]
    private void NextPage()
    {
        if (CurrentPage < TotalPages)
        {
            CurrentPage++;
            UpdatePageInfo();
            UpdateFollowUpGroups();
        }
    }

    [RelayCommand]
    private void MarkAsAcknowledged(FollowUpItem item)
    {
        if (item != null)
        {
            item.Status = FollowUpStatus.Acknowledged;
            UpdateStatistics();
            UpdateFollowUpGroups();
        }
    }

    [RelayCommand]
    private void MarkAsCompleted(FollowUpItem item)
    {
        if (item != null)
        {
            item.Status = FollowUpStatus.Completed;
            UpdateStatistics();
            UpdateFollowUpGroups();
        }
    }

    private void UpdateStatistics()
    {
        PendingCount = _allItems.Count(i => i.Status == FollowUpStatus.Pending);
        AcknowledgedCount = _allItems.Count(i => i.Status == FollowUpStatus.Acknowledged);
    }

    private void UpdatePageInfo()
    {
        int start = (CurrentPage - 1) * PageSize + 1;
        int end = Math.Min(CurrentPage * PageSize, _allItems.Count);
        PageInfo = $"第 {start}-{end} 项，共 {_allItems.Count} 项";
    }

    private void UpdateFollowUpGroups()
    {
        var pagedItems = _allItems
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize)
            .GroupBy(i => i.FollowUpReason)
            .Select(g => new FollowUpGroup
            {
                Reason = g.Key,
                Items = new ObservableCollection<FollowUpItem>(g)
            })
            .ToList();

        FollowUpGroups = new ObservableCollection<FollowUpGroup>(pagedItems);
    }
}

public class FollowUpItem
{
    public Creator Creator { get; set; } = null!;
    public List<Shipment> Shipments { get; set; } = new();
    public List<Video> Videos { get; set; } = new();
    public Relationship? Relationship { get; set; }
    public List<CreatorContact> Contacts { get; set; } = new();
    public string FollowUpReason { get; set; } = "";
    public FollowUpStatus Status { get; set; } = FollowUpStatus.Pending;
}

public class FollowUpGroup
{
    public string Reason { get; set; } = "";
    public ObservableCollection<FollowUpItem> Items { get; set; } = new();
}

public enum FollowUpStatus
{
    Pending,
    Acknowledged,
    Completed
}
