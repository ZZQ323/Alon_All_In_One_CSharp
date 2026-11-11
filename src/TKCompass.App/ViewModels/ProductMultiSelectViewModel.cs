using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TKCompass.Domain.Models;
using TKCompass.Domain.Services;

namespace TKCompass.ViewModels;

public partial class ProductMultiSelectViewModel : ObservableObject
{
    private readonly IProductCodeService _productCodeService;

    [ObservableProperty]
    private ObservableCollection<ProductCode> _availableProducts = new();

    [ObservableProperty]
    private ObservableCollection<ProductCode> _selectedProducts = new();

    [ObservableProperty]
    private ProductCode? _selectedAvailableProduct;

    [ObservableProperty]
    private ProductCode? _selectedSelectedProduct;

    [ObservableProperty]
    private bool _isLoading;

    public ProductMultiSelectViewModel(IProductCodeService productCodeService)
    {
        _productCodeService = productCodeService;
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            var products = await _productCodeService.GetAllAsync();
            AvailableProducts = new ObservableCollection<ProductCode>(products);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void AddSelected()
    {
        if (SelectedAvailableProduct != null && !SelectedProducts.Contains(SelectedAvailableProduct))
        {
            SelectedProducts.Add(SelectedAvailableProduct);
        }
    }

    [RelayCommand]
    private void RemoveSelected()
    {
        if (SelectedSelectedProduct != null)
        {
            SelectedProducts.Remove(SelectedSelectedProduct);
        }
    }

    [RelayCommand]
    private void AddAll()
    {
        foreach (var product in AvailableProducts)
        {
            if (!SelectedProducts.Contains(product))
            {
                SelectedProducts.Add(product);
            }
        }
    }

    [RelayCommand]
    private void RemoveAll()
    {
        SelectedProducts.Clear();
    }
}
