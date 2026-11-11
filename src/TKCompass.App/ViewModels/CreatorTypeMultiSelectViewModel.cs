using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TKCompass.Domain.Models;
using TKCompass.Domain.Services;

namespace TKCompass.ViewModels;

public partial class CreatorTypeMultiSelectViewModel : ObservableObject
{
    private readonly ICreatorTypeService _creatorTypeService;

    [ObservableProperty]
    private ObservableCollection<CreatorType> _availableTypes = new();

    [ObservableProperty]
    private ObservableCollection<CreatorType> _selectedTypes = new();

    [ObservableProperty]
    private CreatorType? _selectedAvailableType;

    [ObservableProperty]
    private CreatorType? _selectedSelectedType;

    [ObservableProperty]
    private bool _isLoading;

    public CreatorTypeMultiSelectViewModel(ICreatorTypeService creatorTypeService)
    {
        _creatorTypeService = creatorTypeService;
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            var types = await _creatorTypeService.GetAllAsync();
            AvailableTypes = new ObservableCollection<CreatorType>(types);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void AddSelected()
    {
        if (SelectedAvailableType != null && !SelectedTypes.Contains(SelectedAvailableType))
        {
            SelectedTypes.Add(SelectedAvailableType);
        }
    }

    [RelayCommand]
    private void RemoveSelected()
    {
        if (SelectedSelectedType != null)
        {
            SelectedTypes.Remove(SelectedSelectedType);
        }
    }

    [RelayCommand]
    private void AddAll()
    {
        foreach (var type in AvailableTypes)
        {
            if (!SelectedTypes.Contains(type))
            {
                SelectedTypes.Add(type);
            }
        }
    }

    [RelayCommand]
    private void RemoveAll()
    {
        SelectedTypes.Clear();
    }
}
