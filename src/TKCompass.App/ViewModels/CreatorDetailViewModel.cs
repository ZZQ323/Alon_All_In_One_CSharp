using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TKCompass.Domain.Models;
using TKCompass.Domain.Services;
using TKCompass.Services;

namespace TKCompass.ViewModels;

public partial class CreatorDetailViewModel : ObservableObject, IRecipient<CreatorUpdatedMessage>
{
    private readonly ICreatorService _creatorService;
    private readonly IShipmentService _shipmentService;
    private readonly IVideoService _videoService;
    private readonly ICreatorContactService _contactService;
    private readonly IProductCodeService _productCodeService;
    private readonly IMessenger _messenger;
    private readonly INotificationService? _notificationService;
    private long _currentCreatorId;
    
    [ObservableProperty]
    private Creator? _creator;
    
    [ObservableProperty]
    private ObservableCollection<Shipment> _shipments = new();
    
    [ObservableProperty]
    private ObservableCollection<Video> _videos = new();
    
    [ObservableProperty]
    private ObservableCollection<CreatorContact> _contacts = new();
    
    [ObservableProperty]
    private bool _isLoading;
    
    [ObservableProperty]
    private bool _hasNoShipments = true;
    
    [ObservableProperty]
    private bool _hasNoVideos = true;
    
    [ObservableProperty]
    private bool _hasNoContacts = true;
    
    /// <summary>请求 View 层弹出寄样编辑窗口</summary>
    public event Action<ShipmentEditViewModel>? OpenShipmentEditRequested;
    
    /// <summary>请求 View 层弹出视频编辑窗口</summary>
    public event Action<VideoEditViewModel>? OpenVideoEditRequested;
    
    public CreatorDetailViewModel(
        ICreatorService creatorService,
        IShipmentService shipmentService,
        IVideoService videoService,
        ICreatorContactService contactService,
        IProductCodeService productCodeService,
        IMessenger messenger,
        INotificationService? notificationService = null)
    {
        _creatorService = creatorService;
        _shipmentService = shipmentService;
        _videoService = videoService;
        _contactService = contactService;
        _productCodeService = productCodeService;
        _messenger = messenger;
        _notificationService = notificationService;
        _messenger.Register(this);
    }
    
    public void Receive(CreatorUpdatedMessage message)
    {
        // 当子窗口保存后发送消息，刷新详情数据
        if (_currentCreatorId > 0)
            _ = LoadCreatorAsync(_currentCreatorId);
    }
    
    public async Task LoadCreatorAsync(long creatorId)
    {
        _currentCreatorId = creatorId;
        IsLoading = true;
        try
        {
            Creator = await _creatorService.GetByIdAsync(creatorId);
            if (Creator != null)
            {
                var shipments = await _shipmentService.GetByCreatorIdAsync(creatorId);
                Shipments = new ObservableCollection<Shipment>(shipments);
                Shipments.CollectionChanged += (s, e) => HasNoShipments = Shipments.Count == 0;
                HasNoShipments = Shipments.Count == 0;
                
                var videos = await _videoService.GetByCreatorIdAsync(creatorId);
                Videos = new ObservableCollection<Video>(videos);
                Videos.CollectionChanged += (s, e) => HasNoVideos = Videos.Count == 0;
                HasNoVideos = Videos.Count == 0;
                
                var contacts = await _contactService.GetByCreatorIdAsync(creatorId);
                Contacts = new ObservableCollection<CreatorContact>(contacts);
                Contacts.CollectionChanged += (s, e) => HasNoContacts = Contacts.Count == 0;
                HasNoContacts = Contacts.Count == 0;
            }
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    [RelayCommand]
    private async Task SaveAsync()
    {
        if (Creator == null) return;
        
        await _creatorService.UpdateAsync(Creator);
    }
    
    [RelayCommand]
    private void AddShipment()
    {
        if (Creator == null) return;
        
        // 实例化 ShipmentEditViewModel 并触发事件请求弹出编辑窗口
        var vm = new ShipmentEditViewModel(_shipmentService, _productCodeService, _messenger)
        {
            IsNew = true,
            CreatorId = Creator.Id,
            SampleDate = DateOnly.FromDateTime(DateTime.Now),
            RequiredVideoCount = 3
        };
        
        OpenShipmentEditRequested?.Invoke(vm);
    }
    
    [RelayCommand]
    private void EditShipment(Shipment shipment)
    {
        if (shipment == null) return;
        
        // 编辑现有寄样
        var vm = new ShipmentEditViewModel(_shipmentService, _productCodeService, _messenger)
        {
            IsNew = false,
            Shipment = shipment
        };
        
        OpenShipmentEditRequested?.Invoke(vm);
    }
    
    [RelayCommand]
    private async Task DeleteShipmentAsync(Shipment shipment)
    {
        if (_notificationService != null)
        {
            var confirmed = await _notificationService.ConfirmAsync(
                "确认删除", 
                $"确定要删除寄样记录 #{shipment.Id} 吗？");
            if (!confirmed)
                return;
        }

        try
        {
            await _shipmentService.DeleteAsync(shipment.Id);
            Shipments.Remove(shipment);
            
            if (_notificationService != null)
                await _notificationService.ShowInfoAsync("成功", "寄样记录已删除");
        }
        catch (Exception ex)
        {
            if (_notificationService != null)
                await _notificationService.ShowErrorAsync("删除失败", ex.Message);
        }
    }
    
    [RelayCommand]
    private void AddVideo(Shipment? shipment = null)
    {
        if (Creator == null) return;
        
        // 实例化 VideoEditViewModel 并触发事件请求弹出编辑窗口
        var vm = new VideoEditViewModel(_videoService, _messenger)
        {
            IsNew = true,
            CreatorId = Creator.Id,
            ShipmentId = shipment?.Id ?? 0,
            VideoDate = DateTime.Now
        };
        
        OpenVideoEditRequested?.Invoke(vm);
    }
    
    [RelayCommand]
    private void EditVideo(Video video)
    {
        if (video == null) return;
        
        // 编辑现有视频
        var vm = new VideoEditViewModel(_videoService, _messenger)
        {
            IsNew = false,
            CreatorId = Creator?.Id ?? 0,
            ShipmentId = video.ShipmentId
        };
        vm.LoadFromVideo(video);
        
        OpenVideoEditRequested?.Invoke(vm);
    }
    
    [RelayCommand]
    private async Task DeleteVideoAsync(Video video)
    {
        if (video == null) return;
        
        if (_notificationService != null)
        {
            var confirmed = await _notificationService.ConfirmAsync(
                "确认删除", 
                $"确定要删除视频记录 #{video.Id} 吗？");
            if (!confirmed)
                return;
        }

        try
        {
            await _videoService.DeleteAsync(video.Id);
            Videos.Remove(video);
            
            if (_notificationService != null)
                await _notificationService.ShowInfoAsync("成功", "视频记录已删除");
        }
        catch (Exception ex)
        {
            if (_notificationService != null)
                await _notificationService.ShowErrorAsync("删除失败", ex.Message);
        }
    }
    
    [RelayCommand]
    private async Task AddContactAsync(string contactType)
    {
        if (Creator == null || string.IsNullOrWhiteSpace(contactType)) return;
        
        // 这里简化处理，实际应该弹出输入框让用户输入联系方式值
        // 暂时使用空字符串，后续可以通过弹窗或其他方式完善
        var type = ParseContactType(contactType);
        var contact = await _contactService.AddContactAsync(Creator.Id, type, "");
        Contacts.Add(contact);
    }
    
    [RelayCommand]
    private async Task DeleteContactAsync(CreatorContact contact)
    {
        if (contact == null) return;
        
        await _contactService.DeleteAsync(contact.Id);
        Contacts.Remove(contact);
    }
    
    private TKCompass.Domain.Enums.ContactType ParseContactType(string type)
    {
        return type switch
        {
            "WhatsApp" => TKCompass.Domain.Enums.ContactType.WhatsApp,
            "Email" => TKCompass.Domain.Enums.ContactType.Email,
            "TikTok后台" => TKCompass.Domain.Enums.ContactType.TiktokBackend,
            "达人后台" => TKCompass.Domain.Enums.ContactType.InfluencerBackend,
            _ => TKCompass.Domain.Enums.ContactType.Other
        };
    }
}
