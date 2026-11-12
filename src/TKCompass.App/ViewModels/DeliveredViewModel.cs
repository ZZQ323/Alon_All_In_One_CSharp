using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TKCompass.Domain.Models;
using TKCompass.Domain.Services;

namespace TKCompass.ViewModels;

public partial class DeliveredViewModel : ObservableObject, IRecipient<CreatorUpdatedMessage>
{
    private readonly ICreatorService _creatorService;
    private readonly IMessenger _messenger;
    public string Title => "已交付";
    
    [ObservableProperty] private ObservableCollection<Creator> creators = new();
    [ObservableProperty] private bool _isLoading;
    
    public DeliveredViewModel(ICreatorService creatorService, IMessenger messenger)
    {
        _creatorService = creatorService;
        _messenger = messenger;
        _messenger.Register(this);
        _ = LoadAsync();
    }
    
    public void Receive(CreatorUpdatedMessage message)
    {
        _ = LoadAsync();
    }
    
    public async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            var all = await _creatorService.GetAllAsync();
            Creators = new ObservableCollection<Creator>(all.Where(c => !c.Deleted && c.Delivered));
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    [RelayCommand]
    private async Task RefreshAsync() => await LoadAsync();
}
