using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Input.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TKCompass.Domain.Models;
using TKCompass.Domain.Services;

namespace TKCompass.ViewModels;

public partial class InDeliveryViewModel : ObservableObject, IRecipient<CreatorUpdatedMessage>
{
    private readonly ICreatorService _creatorService;
    private readonly IMessenger _messenger;

    public string Title => "正在交付";

    [ObservableProperty] private ObservableCollection<Creator> creators = new();
    [ObservableProperty] private ObservableCollection<Creator> selectedCreators = new();

    public InDeliveryViewModel(ICreatorService creatorService, IMessenger messenger)
    {
        _creatorService = creatorService;
        _messenger = messenger;
        _messenger.Register(this);
    }

    public void Receive(CreatorUpdatedMessage message)
    {
        _ = LoadAsync();
    }

    public async Task LoadAsync()
    {
        var all = await _creatorService.GetAllAsync();
        Creators = new ObservableCollection<Creator>(all.Where(c => !c.Deleted && !c.Delivered));
    }

    [RelayCommand]
    private async Task RefreshAsync() => await LoadAsync();
    
    [RelayCommand]
    private async Task CopyHandleAsync()
    {
        if (SelectedCreators.Count == 0) return;
        var text = string.Join("\n", SelectedCreators.Select(c => c.Handle));
        await CopyToClipboardAsync(text);
    }
    
    [RelayCommand]
    private async Task CopyTrackingNumberAsync()
    {
        if (SelectedCreators.Count == 0) return;
        var trackingNumbers = SelectedCreators
            .Select(c => c.Shipments?.FirstOrDefault()?.TrackingNumber)
            .Where(t => !string.IsNullOrEmpty(t));
        var text = string.Join("\n", trackingNumbers);
        await CopyToClipboardAsync(text);
    }
    
    private static async Task CopyToClipboardAsync(string text)
    {
        if (string.IsNullOrEmpty(text)) return;
        
        var lifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        var topLevel = lifetime?.MainWindow as TopLevel;
        var clipboard = topLevel?.Clipboard;
        if (clipboard != null)
        {
            await clipboard.SetTextAsync(text);
        }
    }
}
