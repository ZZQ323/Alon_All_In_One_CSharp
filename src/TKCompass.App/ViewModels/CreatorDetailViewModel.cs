using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TKCompass.Domain.Models;
using TKCompass.Domain.Services;

namespace TKCompass.ViewModels;

public partial class CreatorDetailViewModel : ObservableObject
{
    private readonly ICreatorService _creatorService;
    private readonly IShipmentService _shipmentService;
    private readonly IVideoService _videoService;
    private readonly ICreatorContactService _contactService;
    
    [ObservableProperty]
    private Creator? _creator;
    
    [ObservableProperty]
    private ObservableCollection<Shipment> _shipments = new();
    
    [ObservableProperty]
    private ObservableCollection<Video> _videos = new();
    
    [ObservableProperty]
    private bool _isLoading;
    
    public CreatorDetailViewModel(
        ICreatorService creatorService,
        IShipmentService shipmentService,
        IVideoService videoService,
        ICreatorContactService contactService)
    {
        _creatorService = creatorService;
        _shipmentService = shipmentService;
        _videoService = videoService;
        _contactService = contactService;
    }
    
    public async Task LoadCreatorAsync(long creatorId)
    {
        IsLoading = true;
        try
        {
            Creator = await _creatorService.GetByIdAsync(creatorId);
            if (Creator != null)
            {
                var shipments = await _shipmentService.GetByCreatorIdAsync(creatorId);
                Shipments = new ObservableCollection<Shipment>(shipments);
                
                var videos = await _videoService.GetByCreatorIdAsync(creatorId);
                Videos = new ObservableCollection<Video>(videos);
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
    private async Task AddShipmentAsync()
    {
        if (Creator == null) return;
        
        var shipment = new Shipment
        {
            CreatorId = Creator.Id,
            SampleDate = DateOnly.FromDateTime(DateTime.Now),
            RequiredVideoCount = 3
        };
        
        var saved = await _shipmentService.CreateShipmentAsync(
            Creator.Id, 
            shipment.TrackingNumber ?? "", 
            shipment.SampleDate ?? DateOnly.FromDateTime(DateTime.Now));
        
        Shipments.Add(saved);
    }
    
    [RelayCommand]
    private async Task DeleteShipmentAsync(Shipment shipment)
    {
        await _shipmentService.DeleteAsync(shipment.Id);
        Shipments.Remove(shipment);
    }
    
    [RelayCommand]
    private async Task AddVideoAsync(Shipment shipment)
    {
        if (Creator == null) return;
        
        var video = new Video
        {
            CreatorId = Creator.Id,
            ShipmentId = shipment.Id,
            VideoDate = DateTime.Now
        };
        
        var saved = await _videoService.CreateVideoAsync(Creator.Id, video.VideoUrl ?? "");
        Videos.Add(saved);
    }
}
