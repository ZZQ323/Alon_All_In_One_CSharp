using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TKCompass.Domain.Models;
using TKCompass.Domain.Services;

namespace TKCompass.ViewModels;

/// <summary>
/// 配置管理 ViewModel
/// </summary>
public partial class ConfigViewModel : ObservableObject
{
    private readonly ICreatorTypeService _creatorTypeService;
    private readonly IProductCodeService _productCodeService;
    private readonly IDictionaryService _dictionaryService;
    private readonly IMessenger _messenger;

    [ObservableProperty]
    private ObservableCollection<CreatorType> _creatorTypes = new();

    [ObservableProperty]
    private ObservableCollection<ProductCode> _productCodes = new();

    [ObservableProperty]
    private ObservableCollection<Dictionary> _qualityLevels = new();

    [ObservableProperty]
    private ObservableCollection<Dictionary> _contactChannels = new();

    [ObservableProperty]
    private CreatorType? _selectedCreatorType;

    [ObservableProperty]
    private ProductCode? _selectedProductCode;

    [ObservableProperty]
    private Dictionary? _selectedQualityLevel;

    [ObservableProperty]
    private Dictionary? _selectedContactChannel;

    // 达人类型新增字段
    [ObservableProperty]
    private string _newCreatorTypeName = "";

    [ObservableProperty]
    private string _newCreatorTypeDescription = "";

    // 产品代码新增字段
    [ObservableProperty]
    private string _newProductCode = "";

    [ObservableProperty]
    private string _newProductName = "";

    [ObservableProperty]
    private string _newProductDescription = "";

    // 质量等级新增字段
    [ObservableProperty]
    private string _newQualityCode = "";

    [ObservableProperty]
    private string _newQualityLabel = "";

    [ObservableProperty]
    private int _newQualitySort = 0;

    // 联系方式渠道新增字段
    [ObservableProperty]
    private string _newChannelCode = "";

    [ObservableProperty]
    private string _newChannelLabel = "";

    [ObservableProperty]
    private int _newChannelSort = 0;

    [ObservableProperty]
    private string _errorMessage = "";

    [ObservableProperty]
    private bool _hasError;

    public ConfigViewModel(
        ICreatorTypeService creatorTypeService,
        IProductCodeService productCodeService,
        IDictionaryService dictionaryService,
        IMessenger messenger)
    {
        _creatorTypeService = creatorTypeService;
        _productCodeService = productCodeService;
        _dictionaryService = dictionaryService;
        _messenger = messenger;
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        HasError = false;
        ErrorMessage = "";
        try
        {
            var types = await _creatorTypeService.GetAllAsync();
            CreatorTypes = new ObservableCollection<CreatorType>(types);

            var codes = await _productCodeService.GetAllAsync();
            ProductCodes = new ObservableCollection<ProductCode>(codes);

            var levels = await _dictionaryService.GetByTypeAsync("quality_level");
            QualityLevels = new ObservableCollection<Dictionary>(levels.OrderBy(l => l.SortOrder));

            var channels = await _dictionaryService.GetByTypeAsync("contact_channel");
            ContactChannels = new ObservableCollection<Dictionary>(channels.OrderBy(c => c.SortOrder));
        }
        catch (Exception ex)
        {
            ErrorMessage = $"加载配置失败: {ex.Message}";
            HasError = true;
        }
    }

    #region 达人类型管理

    [RelayCommand]
    private async Task AddCreatorTypeAsync()
    {
        if (string.IsNullOrWhiteSpace(NewCreatorTypeName))
        {
            ErrorMessage = "达人类型名称不能为空";
            HasError = true;
            return;
        }

        try
        {
            var type = new CreatorType
            {
                Name = NewCreatorTypeName.Trim(),
                Description = string.IsNullOrWhiteSpace(NewCreatorTypeDescription) ? null : NewCreatorTypeDescription.Trim(),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            var saved = await _creatorTypeService.SaveAsync(type);
            CreatorTypes.Add(saved);
            NewCreatorTypeName = "";
            NewCreatorTypeDescription = "";
            HasError = false;
            _messenger.Send(new CreatorUpdatedMessage());
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            HasError = true;
        }
    }

    [RelayCommand]
    private async Task DeleteCreatorTypeAsync()
    {
        if (SelectedCreatorType == null) return;

        try
        {
            await _creatorTypeService.DeleteAsync(SelectedCreatorType.Id);
            CreatorTypes.Remove(SelectedCreatorType);
            SelectedCreatorType = null;
            _messenger.Send(new CreatorUpdatedMessage());
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            HasError = true;
        }
    }

    #endregion

    #region 产品代码管理

    [RelayCommand]
    private async Task AddProductCodeAsync()
    {
        if (string.IsNullOrWhiteSpace(NewProductCode))
        {
            ErrorMessage = "产品代码不能为空";
            HasError = true;
            return;
        }

        if (string.IsNullOrWhiteSpace(NewProductName))
        {
            ErrorMessage = "产品名称不能为空";
            HasError = true;
            return;
        }

        try
        {
            var code = new ProductCode
            {
                Code = NewProductCode.Trim(),
                Name = NewProductName.Trim(),
                Description = string.IsNullOrWhiteSpace(NewProductDescription) ? null : NewProductDescription.Trim(),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            var saved = await _productCodeService.SaveAsync(code);
            ProductCodes.Add(saved);
            NewProductCode = "";
            NewProductName = "";
            NewProductDescription = "";
            HasError = false;
            _messenger.Send(new CreatorUpdatedMessage());
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            HasError = true;
        }
    }

    [RelayCommand]
    private async Task DeleteProductCodeAsync()
    {
        if (SelectedProductCode == null) return;

        try
        {
            await _productCodeService.DeleteAsync(SelectedProductCode.Id);
            ProductCodes.Remove(SelectedProductCode);
            SelectedProductCode = null;
            _messenger.Send(new CreatorUpdatedMessage());
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            HasError = true;
        }
    }

    #endregion

    #region 质量等级管理

    [RelayCommand]
    private async Task AddQualityLevelAsync()
    {
        if (string.IsNullOrWhiteSpace(NewQualityCode))
        {
            ErrorMessage = "等级代码不能为空";
            HasError = true;
            return;
        }

        if (string.IsNullOrWhiteSpace(NewQualityLabel))
        {
            ErrorMessage = "等级标签不能为空";
            HasError = true;
            return;
        }

        try
        {
            var level = new Dictionary
            {
                Type = "quality_level",
                Code = NewQualityCode.Trim(),
                Label = NewQualityLabel.Trim(),
                SortOrder = NewQualitySort,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            var saved = await _dictionaryService.SaveAsync(level);
            QualityLevels.Add(saved);
            QualityLevels = new ObservableCollection<Dictionary>(QualityLevels.OrderBy(l => l.SortOrder));
            NewQualityCode = "";
            NewQualityLabel = "";
            NewQualitySort = 0;
            HasError = false;
            _messenger.Send(new CreatorUpdatedMessage());
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            HasError = true;
        }
    }

    [RelayCommand]
    private async Task DeleteQualityLevelAsync()
    {
        if (SelectedQualityLevel == null) return;

        try
        {
            await _dictionaryService.DeleteAsync(SelectedQualityLevel.Type, SelectedQualityLevel.Code);
            QualityLevels.Remove(SelectedQualityLevel);
            SelectedQualityLevel = null;
            _messenger.Send(new CreatorUpdatedMessage());
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            HasError = true;
        }
    }

    #endregion

    #region 联系方式渠道管理

    [RelayCommand]
    private async Task AddContactChannelAsync()
    {
        if (string.IsNullOrWhiteSpace(NewChannelCode))
        {
            ErrorMessage = "渠道代码不能为空";
            HasError = true;
            return;
        }

        if (string.IsNullOrWhiteSpace(NewChannelLabel))
        {
            ErrorMessage = "渠道标签不能为空";
            HasError = true;
            return;
        }

        try
        {
            var channel = new Dictionary
            {
                Type = "contact_channel",
                Code = NewChannelCode.Trim(),
                Label = NewChannelLabel.Trim(),
                SortOrder = NewChannelSort,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            var saved = await _dictionaryService.SaveAsync(channel);
            ContactChannels.Add(saved);
            ContactChannels = new ObservableCollection<Dictionary>(ContactChannels.OrderBy(c => c.SortOrder));
            NewChannelCode = "";
            NewChannelLabel = "";
            NewChannelSort = 0;
            HasError = false;
            _messenger.Send(new CreatorUpdatedMessage());
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            HasError = true;
        }
    }

    [RelayCommand]
    private async Task DeleteContactChannelAsync()
    {
        if (SelectedContactChannel == null) return;

        try
        {
            await _dictionaryService.DeleteAsync(SelectedContactChannel.Type, SelectedContactChannel.Code);
            ContactChannels.Remove(SelectedContactChannel);
            SelectedContactChannel = null;
            _messenger.Send(new CreatorUpdatedMessage());
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            HasError = true;
        }
    }

    #endregion

    [RelayCommand]
    private async Task RefreshAsync()
    {
        await LoadDataAsync();
    }
}
