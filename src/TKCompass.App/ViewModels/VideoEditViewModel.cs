using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TKCompass.Domain.Models;
using TKCompass.Domain.Services;

namespace TKCompass.ViewModels;

public partial class VideoEditViewModel : ObservableObject
{
    private readonly IVideoService _videoService;
    private readonly IMessenger _messenger;
    
    [ObservableProperty]
    private Video? _video;
    
    [ObservableProperty]
    private string _videoUrl = "";
    
    [ObservableProperty]
    private DateTime? _videoDate;
    
    [ObservableProperty]
    private string? _adCode;
    
    [ObservableProperty]
    private int _views;
    
    [ObservableProperty]
    private int _orders;
    
    [ObservableProperty]
    private decimal _gmv;
    
    [ObservableProperty]
    private bool _isEffective;
    
    public bool IsNew { get; set; }
    public long CreatorId { get; set; }
    public long? ShipmentId { get; set; }
    
    public VideoEditViewModel(IVideoService videoService, IMessenger messenger)
    {
        _videoService = videoService;
        _messenger = messenger;
        _videoDate = DateTime.Now;
    }
    
    public void LoadFromVideo(Video video)
    {
        Video = video;
        VideoUrl = video.VideoUrl ?? "";
        VideoDate = video.VideoDate;
        AdCode = video.AdCode;
        Views = video.Views;
        Orders = video.Orders;
        Gmv = video.Gmv;
        IsEffective = video.IsEffective;
    }
    
    [RelayCommand]
    private async Task SaveAsync()
    {
        if (IsNew)
        {
            var created = await _videoService.CreateVideoAsync(CreatorId, VideoUrl);
            created.ShipmentId = ShipmentId;
            created.VideoDate = VideoDate;
            created.AdCode = AdCode;
            created.Views = Views;
            created.Orders = Orders;
            created.Gmv = Gmv;
            created.IsEffective = IsEffective;
            await _videoService.UpdateAsync(created);
            Video = created;
        }
        else if (Video != null)
        {
            Video.VideoUrl = VideoUrl;
            Video.VideoDate = VideoDate;
            Video.AdCode = AdCode;
            Video.Views = Views;
            Video.Orders = Orders;
            Video.Gmv = Gmv;
            Video.IsEffective = IsEffective;
            await _videoService.UpdateAsync(Video);
        }
        
        // 发送更新消息
        _messenger.Send(new CreatorUpdatedMessage());
    }
}
