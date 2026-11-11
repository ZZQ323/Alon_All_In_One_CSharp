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
    
    // 全量数据缓存，用于筛选
    private List<Creator> _allCreators = new();

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
            _allCreators = allCreators.ToList(); // 缓存全量数据
            FilterCreators(); // 应用当前筛选条件
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
        var filtered = _allCreators.AsEnumerable();
        
        // 按等级筛选（QualityLevel = S/A/B/C/D）
        if (!string.IsNullOrEmpty(SelectedLevel) && SelectedLevel != "全部")
        {
            filtered = filtered.Where(c => c.QualityLevel == SelectedLevel);
        }
        
        // 按搜索文本筛选（匹配昵称、备注、Handle）
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var search = SearchText.Trim().ToLower();
            filtered = filtered.Where(c => 
                (c.Nickname?.ToLower().Contains(search) ?? false) ||
                (c.Notes?.ToLower().Contains(search) ?? false) ||
                (c.Handle?.ToLower().Contains(search) ?? false));
        }
        
        Creators = new ObservableCollection<Creator>(filtered);
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        await LoadDataAsync();
    }
}
