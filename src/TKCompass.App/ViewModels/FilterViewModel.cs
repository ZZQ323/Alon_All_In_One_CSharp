using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using TKCompass.Domain.Enums;

namespace TKCompass.ViewModels;

public partial class FilterViewModel : ObservableObject
{
    [ObservableProperty] private string _handleSearch = "";
    [ObservableProperty] private string _selectedStatus = "";
    [ObservableProperty] private string _selectedQualityLevel = "";
    [ObservableProperty] private DateOnly? _sampleDateFrom;
    [ObservableProperty] private DateOnly? _sampleDateTo;
    [ObservableProperty] private DateOnly? _contactDateFrom;
    [ObservableProperty] private DateOnly? _contactDateTo;
    [ObservableProperty] private ObservableCollection<string> _selectedProductCodes = new();
    [ObservableProperty] private string _productNameSearch = "";
    
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
