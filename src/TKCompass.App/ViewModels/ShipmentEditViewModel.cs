using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TKCompass.Domain.Models;
using TKCompass.Domain.Services;

namespace TKCompass.ViewModels;

public partial class ShipmentEditViewModel : ObservableObject
{
    private readonly IShipmentService _shipmentService;
    private readonly IProductCodeService _productCodeService;
    private readonly IMessenger _messenger;
    
    [ObservableProperty]
    private Shipment? _shipment;
    
    [ObservableProperty]
    private string _trackingNumber = "";
    
    [ObservableProperty]
    private DateOnly _sampleDate;
    
    [ObservableProperty]
    private string _sku = "";
    
    [ObservableProperty]
    private string _productName = "";
    
    [ObservableProperty]
    private int _requiredVideoCount = 3;
    
    [ObservableProperty]
    private string _notes = "";
    
    [ObservableProperty]
    private bool _isCompletedManual;
    
    [ObservableProperty]
    private ObservableCollection<ProductCode> _availableProductCodes = new();
    
    public bool IsNew { get; set; }
    public long CreatorId { get; set; }
    
    public ShipmentEditViewModel(
        IShipmentService shipmentService,
        IProductCodeService productCodeService,
        IMessenger messenger)
    {
        _shipmentService = shipmentService;
        _productCodeService = productCodeService;
        _messenger = messenger;
        _sampleDate = DateOnly.FromDateTime(DateTime.Now);
    }
    
    public async Task LoadAsync()
    {
        var codes = await _productCodeService.GetAllAsync();
        AvailableProductCodes = new ObservableCollection<ProductCode>(codes);
        
        if (Shipment != null)
        {
            TrackingNumber = Shipment.TrackingNumber ?? "";
            SampleDate = Shipment.SampleDate ?? DateOnly.FromDateTime(DateTime.Now);
            Sku = Shipment.Sku ?? "";
            ProductName = Shipment.ProductName ?? "";
            RequiredVideoCount = Shipment.RequiredVideoCount;
            Notes = Shipment.Notes ?? "";
            IsCompletedManual = Shipment.IsCompletedManual;
        }
    }
    
    [RelayCommand]
    private async Task SaveAsync()
    {
        if (IsNew)
        {
            var created = await _shipmentService.CreateShipmentAsync(CreatorId, TrackingNumber, SampleDate);
            created.Sku = Sku;
            created.ProductName = ProductName;
            created.RequiredVideoCount = RequiredVideoCount;
            created.Notes = Notes;
            await _shipmentService.UpdateAsync(created);
            Shipment = created;
        }
        else if (Shipment != null)
        {
            Shipment.TrackingNumber = TrackingNumber;
            Shipment.SampleDate = SampleDate;
            Shipment.Sku = Sku;
            Shipment.ProductName = ProductName;
            Shipment.RequiredVideoCount = RequiredVideoCount;
            Shipment.Notes = Notes;
            Shipment.IsCompletedManual = IsCompletedManual;
            await _shipmentService.UpdateAsync(Shipment);
        }
        
        // 发送更新消息
        _messenger.Send(new CreatorUpdatedMessage());
    }
}
