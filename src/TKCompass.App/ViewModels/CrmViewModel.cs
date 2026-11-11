using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TKCompass.Domain.Models;
using TKCompass.Domain.Services;

namespace TKCompass.ViewModels;

public partial class CrmViewModel : ObservableObject, IRecipient<CreatorUpdatedMessage>
{
    private readonly ICreatorService _creatorService;
    private readonly IMessenger _messenger;

    [ObservableProperty]
    private ObservableCollection<Creator> _creators = new();

    [ObservableProperty]
    private Creator? _selectedCreator;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private string _selectedLevel = "全部";

    [ObservableProperty]
    private bool _isLoading;

    public ObservableCollection<string> Levels { get; } = new() { "全部", "S", "A", "B", "C", "D" };

    public CrmViewModel(ICreatorService creatorService, IMessenger messenger)
    {
        _creatorService = creatorService;
        _messenger = messenger;
        _messenger.Register(this);
        _ = LoadDataAsync();
    }

    public void Receive(CreatorUpdatedMessage message)
    {
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            var allCreators = await _creatorService.GetAllAsync();
            Creators = new ObservableCollection<Creator>(allCreators);
        }
        finally
        {
            IsLoading = false;
        }
    }

    partial void OnSearchTextChanged(string value)
    {
        FilterCreators();
    }

    partial void OnSelectedLevelChanged(string value)
    {
        FilterCreators();
    }

    private void FilterCreators()
    {
        // TODO: 实现筛选逻辑
        // 根据 SearchText 和 SelectedLevel 过滤 Creators
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        await LoadDataAsync();
    }
}
