using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TKCompass.Domain.Models;
using TKCompass.Domain.Services;
using System.Collections.ObjectModel;

namespace TKCompass.ViewModels;

public partial class ShipmentDetailViewModel : ObservableObject
{
    private readonly IShipmentService _shipmentService;
    private readonly IVideoService _videoService;

    [ObservableProperty]
    private Shipment? _shipment;

    [ObservableProperty]
    private ObservableCollection<Video> _videos = new();

    [ObservableProperty]
    private bool _isLoading;

    public ShipmentDetailViewModel(IShipmentService shipmentService, IVideoService videoService)
    {
        _shipmentService = shipmentService;
        _videoService = videoService;
    }

    public async Task LoadShipmentAsync(long shipmentId)
    {
        try
        {
            IsLoading = true;
            Shipment = await _shipmentService.GetByIdAsync(shipmentId);
            
            if (Shipment != null)
            {
                var videos = await _videoService.GetByShipmentIdAsync(shipmentId);
                Videos = new ObservableCollection<Video>(videos);
            }
        }
        finally
        {
            IsLoading = false;
        }
    }
}
