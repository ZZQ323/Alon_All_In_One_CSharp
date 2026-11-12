using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TKCompass.Domain.Models;
using TKCompass.Domain.Services;
using TKCompass.Services;

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
    private readonly INotificationService? _notificationService;

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

    [ObservableProperty]
    private string _errorMessage = "";

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private bool _isLoading;

    /// <summary>
    /// 由 View 注入的弹窗回调：返回 true 表示用户确认，false 表示取消
    /// </summary>
    public Func<ConfigEditContext, Task<bool>>? ShowEditDialogAsync { get; set; }

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
        IsLoading = true;
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
        finally
        {
            IsLoading = false;
        }
    }

    #region 达人类型管理

    [RelayCommand]
    private async Task EditCreatorTypeAsync()
    {
        if (SelectedCreatorType == null) return;

        if (ShowEditDialogAsync != null)
        {
            var context = new ConfigEditContext
            {
                IsEdit = true,
                Title = "编辑达人类型",
                Label = SelectedCreatorType.Name,
                Description = SelectedCreatorType.Description ?? ""
            };
            var result = await ShowEditDialogAsync(context);
            if (result)
            {
                SelectedCreatorType.Name = context.Label;
                SelectedCreatorType.Description = string.IsNullOrWhiteSpace(context.Description) ? null : context.Description;
                SelectedCreatorType.UpdatedAt = DateTime.Now;
                var saved = await _creatorTypeService.SaveAsync(SelectedCreatorType);
                var index = CreatorTypes.IndexOf(SelectedCreatorType);
                if (index >= 0) CreatorTypes[index] = saved;
                _messenger.Send(new CreatorUpdatedMessage());
            }
        }
    }

    [RelayCommand]
    private async Task AddCreatorTypeAsync()
    {
        if (ShowEditDialogAsync != null)
        {
            var context = new ConfigEditContext
            {
                IsEdit = false,
                Title = "新增达人类型",
                Label = "",
                Description = ""
            };
            var result = await ShowEditDialogAsync(context);
            if (result)
            {
                var type = new CreatorType
                {
                    Name = context.Label.Trim(),
                    Description = string.IsNullOrWhiteSpace(context.Description) ? null : context.Description.Trim(),
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                var saved = await _creatorTypeService.SaveAsync(type);
                CreatorTypes.Add(saved);
                _messenger.Send(new CreatorUpdatedMessage());
            }
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
    private async Task EditProductCodeAsync()
    {
        if (SelectedProductCode == null) return;

        if (ShowEditDialogAsync != null)
        {
            var context = new ConfigEditContext
            {
                IsEdit = true,
                Title = "编辑产品代码",
                Code = SelectedProductCode.Code,
                Label = SelectedProductCode.Name,
                Description = SelectedProductCode.Description ?? ""
            };
            var result = await ShowEditDialogAsync(context);
            if (result)
            {
                SelectedProductCode.Code = context.Code;
                SelectedProductCode.Name = context.Label;
                SelectedProductCode.Description = string.IsNullOrWhiteSpace(context.Description) ? null : context.Description;
                SelectedProductCode.UpdatedAt = DateTime.Now;
                var saved = await _productCodeService.SaveAsync(SelectedProductCode);
                var index = ProductCodes.IndexOf(SelectedProductCode);
                if (index >= 0) ProductCodes[index] = saved;
                _messenger.Send(new CreatorUpdatedMessage());
            }
        }
    }

    [RelayCommand]
    private async Task AddProductCodeAsync()
    {
        if (ShowEditDialogAsync != null)
        {
            var context = new ConfigEditContext
            {
                IsEdit = false,
                Title = "新增产品代码",
                ShowCodeField = true,
                Code = "",
                Label = "",
                Description = ""
            };
            var result = await ShowEditDialogAsync(context);
            if (result)
            {
                var code = new ProductCode
                {
                    Code = context.Code.Trim(),
                    Name = context.Label.Trim(),
                    Description = string.IsNullOrWhiteSpace(context.Description) ? null : context.Description.Trim(),
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                var saved = await _productCodeService.SaveAsync(code);
                ProductCodes.Add(saved);
                _messenger.Send(new CreatorUpdatedMessage());
            }
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
    private async Task EditQualityLevelAsync()
    {
        if (SelectedQualityLevel == null) return;

        if (ShowEditDialogAsync != null)
        {
            var context = new ConfigEditContext
            {
                IsEdit = true,
                Title = "编辑质量等级",
                Code = SelectedQualityLevel.Code,
                Label = SelectedQualityLevel.Label,
                SortOrder = SelectedQualityLevel.SortOrder
            };
            var result = await ShowEditDialogAsync(context);
            if (result)
            {
                SelectedQualityLevel.Code = context.Code;
                SelectedQualityLevel.Label = context.Label;
                SelectedQualityLevel.SortOrder = context.SortOrder;
                SelectedQualityLevel.UpdatedAt = DateTime.Now;
                var saved = await _dictionaryService.SaveAsync(SelectedQualityLevel);
                var index = QualityLevels.IndexOf(SelectedQualityLevel);
                if (index >= 0) QualityLevels[index] = saved;
                _messenger.Send(new CreatorUpdatedMessage());
            }
        }
    }

    [RelayCommand]
    private async Task AddQualityLevelAsync()
    {
        if (ShowEditDialogAsync != null)
        {
            var context = new ConfigEditContext
            {
                IsEdit = false,
                Title = "新增质量等级",
                ShowCodeField = true,
                ShowSortOrderField = true,
                Code = "",
                Label = "",
                SortOrder = 0
            };
            var result = await ShowEditDialogAsync(context);
            if (result)
            {
                var level = new Dictionary
                {
                    Type = "quality_level",
                    Code = context.Code.Trim(),
                    Label = context.Label.Trim(),
                    SortOrder = context.SortOrder,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                var saved = await _dictionaryService.SaveAsync(level);
                QualityLevels.Add(saved);
                QualityLevels = new ObservableCollection<Dictionary>(QualityLevels.OrderBy(l => l.SortOrder));
                _messenger.Send(new CreatorUpdatedMessage());
            }
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
    private async Task EditContactChannelAsync()
    {
        if (SelectedContactChannel == null) return;

        if (ShowEditDialogAsync != null)
        {
            var context = new ConfigEditContext
            {
                IsEdit = true,
                Title = "编辑联系方式渠道",
                Code = SelectedContactChannel.Code,
                Label = SelectedContactChannel.Label,
                SortOrder = SelectedContactChannel.SortOrder
            };
            var result = await ShowEditDialogAsync(context);
            if (result)
            {
                SelectedContactChannel.Code = context.Code;
                SelectedContactChannel.Label = context.Label;
                SelectedContactChannel.SortOrder = context.SortOrder;
                SelectedContactChannel.UpdatedAt = DateTime.Now;
                var saved = await _dictionaryService.SaveAsync(SelectedContactChannel);
                var index = ContactChannels.IndexOf(SelectedContactChannel);
                if (index >= 0) ContactChannels[index] = saved;
                _messenger.Send(new CreatorUpdatedMessage());
            }
        }
    }

    [RelayCommand]
    private async Task AddContactChannelAsync()
    {
        if (ShowEditDialogAsync != null)
        {
            var context = new ConfigEditContext
            {
                IsEdit = false,
                Title = "新增联系方式渠道",
                ShowCodeField = true,
                ShowSortOrderField = true,
                Code = "",
                Label = "",
                SortOrder = 0
            };
            var result = await ShowEditDialogAsync(context);
            if (result)
            {
                var channel = new Dictionary
                {
                    Type = "contact_channel",
                    Code = context.Code.Trim(),
                    Label = context.Label.Trim(),
                    SortOrder = context.SortOrder,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                var saved = await _dictionaryService.SaveAsync(channel);
                ContactChannels.Add(saved);
                ContactChannels = new ObservableCollection<Dictionary>(ContactChannels.OrderBy(c => c.SortOrder));
                _messenger.Send(new CreatorUpdatedMessage());
            }
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
