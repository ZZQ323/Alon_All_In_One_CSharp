using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TKCompass.Domain.Enums;

namespace TKCompass.ViewModels;

public partial class FilterViewModel : ObservableObject
{
    // 回调委托 - 由父 ViewModel 注入
    public Action? OnRefresh { get; set; }
    public Action? OnApplyFilter { get; set; }
    public Action? OnClearFilter { get; set; }
    public Action? OnCreateCreator { get; set; }

    [ObservableProperty] private string _handleSearch = "";
    [ObservableProperty] private string _selectedStatus = "";
    [ObservableProperty] private string _selectedQualityLevel = "";
    [ObservableProperty] private DateOnly? _sampleDateFrom;
    [ObservableProperty] private DateOnly? _sampleDateTo;
    [ObservableProperty] private DateOnly? _contactDateFrom;
    [ObservableProperty] private DateOnly? _contactDateTo;
    [ObservableProperty] private ObservableCollection<string> _selectedProductCodes = new();
    [ObservableProperty] private string _productNameSearch = "";

    [RelayCommand]
    private void Refresh()
    {
        OnRefresh?.Invoke();
    }

    [RelayCommand]
    private void ApplyFilter()
    {
        OnApplyFilter?.Invoke();
    }

    [RelayCommand]
    private void ClearFilter()
    {
        OnClearFilter?.Invoke();
    }

    [RelayCommand]
    private void CreateCreator()
    {
        OnCreateCreator?.Invoke();
    }
    
    public ObservableCollection<string> StatusOptions { get; } = new()
    {
        "", "已触达", "已发送合作", "已同意合作", "已寄样", "正在交付", "已交付", "已删除"
    };
    
    public ObservableCollection<string> QualityLevelOptions { get; } = new()
    {
        "", "S", "A", "B", "C", "D"
    };
    
    public ObservableCollection<string> AvailableProductCodes { get; } = new();
    
    public void Reset()
    {
        HandleSearch = "";
        SelectedStatus = "";
        SelectedQualityLevel = "";
        SampleDateFrom = null;
        SampleDateTo = null;
        ContactDateFrom = null;
        ContactDateTo = null;
        SelectedProductCodes.Clear();
        ProductNameSearch = "";
    }
    
    public bool HasActiveFilter =>
        !string.IsNullOrEmpty(HandleSearch) ||
        !string.IsNullOrEmpty(SelectedStatus) ||
        !string.IsNullOrEmpty(SelectedQualityLevel) ||
        SampleDateFrom.HasValue || SampleDateTo.HasValue ||
        ContactDateFrom.HasValue || ContactDateTo.HasValue ||
        SelectedProductCodes.Count > 0 ||
        !string.IsNullOrEmpty(ProductNameSearch);
}
