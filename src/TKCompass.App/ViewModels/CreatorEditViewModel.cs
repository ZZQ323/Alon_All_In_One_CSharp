using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TKCompass.Domain.Enums;
using TKCompass.Domain.Models;
using TKCompass.Domain.Services;

namespace TKCompass.ViewModels;

/// <summary>
/// 达人编辑对话框 ViewModel
/// </summary>
public partial class CreatorEditViewModel : ObservableObject
{
    private readonly ICreatorService _creatorService;
    private readonly IMessenger _messenger;

    [ObservableProperty]
    private long _creatorId;

    [ObservableProperty]
    private string _handle = "";

    [ObservableProperty]
    private string _nickname = "";

    [ObservableProperty]
    private string _qualityLevel = "C";

    [ObservableProperty]
    private string _preferredLanguage = "";

    [ObservableProperty]
    private string _countryRegion = "";

    [ObservableProperty]
    private string _contentCategory = "";

    [ObservableProperty]
    private string _sourceChannel = "";

    [ObservableProperty]
    private string _commissionRate = "";

    [ObservableProperty]
    private string _creatorNotes = "";

    [ObservableProperty]
    private string _tiktokHomepageUrl = "";

    [ObservableProperty]
    private bool _isEditMode;

    [ObservableProperty]
    private string _errorMessage = "";

    [ObservableProperty]
    private bool _hasError;

    public ObservableCollection<string> QualityLevelOptions { get; } = new()
    {
        "S", "A", "B", "C", "D"
    };

    public CreatorEditViewModel(ICreatorService creatorService, IMessenger messenger)
    {
        _creatorService = creatorService;
        _messenger = messenger;
    }

    public async Task LoadCreatorAsync(long creatorId)
    {
        if (creatorId <= 0)
        {
            IsEditMode = false;
            return;
        }

        var creator = await _creatorService.GetByIdAsync(creatorId);
        if (creator == null)
        {
            ErrorMessage = "达人不存在";
            HasError = true;
            return;
        }

        CreatorId = creator.Id;
        Handle = creator.Handle;
        Nickname = creator.Nickname;
        QualityLevel = creator.QualityLevel;
        PreferredLanguage = creator.PreferredLanguage ?? "";
        CountryRegion = creator.CountryRegion ?? "";
        ContentCategory = creator.ContentCategory ?? "";
        SourceChannel = creator.SourceChannel ?? "";
        CommissionRate = creator.CommissionRate ?? "";
        CreatorNotes = creator.CreatorNotes ?? "";
        TiktokHomepageUrl = creator.TiktokHomepageUrl ?? "";
        IsEditMode = true;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        // 验证
        if (string.IsNullOrWhiteSpace(Handle))
        {
            ErrorMessage = "达人ID不能为空";
            HasError = true;
            return;
        }

        if (string.IsNullOrWhiteSpace(Nickname))
        {
            ErrorMessage = "昵称不能为空";
            HasError = true;
            return;
        }

        try
        {
            if (IsEditMode)
            {
                // 编辑模式
                var creator = await _creatorService.GetByIdAsync(CreatorId);
                if (creator == null)
                {
                    ErrorMessage = "达人不存在";
                    HasError = true;
                    return;
                }

                creator.Handle = Handle.Trim();
                creator.Nickname = Nickname.Trim();
                creator.QualityLevel = QualityLevel;
                creator.PreferredLanguage = string.IsNullOrWhiteSpace(PreferredLanguage) ? null : PreferredLanguage.Trim();
                creator.CountryRegion = string.IsNullOrWhiteSpace(CountryRegion) ? null : CountryRegion.Trim();
                creator.ContentCategory = string.IsNullOrWhiteSpace(ContentCategory) ? null : ContentCategory.Trim();
                creator.SourceChannel = string.IsNullOrWhiteSpace(SourceChannel) ? null : SourceChannel.Trim();
                creator.CommissionRate = string.IsNullOrWhiteSpace(CommissionRate) ? null : CommissionRate.Trim();
                creator.CreatorNotes = string.IsNullOrWhiteSpace(CreatorNotes) ? null : CreatorNotes.Trim();
                creator.TiktokHomepageUrl = string.IsNullOrWhiteSpace(TiktokHomepageUrl) ? null : TiktokHomepageUrl.Trim();

                await _creatorService.UpdateAsync(creator);
            }
            else
            {
                // 新增模式
                await _creatorService.CreateCreatorAsync(Handle.Trim(), Nickname.Trim(), QualityLevel);
            }

            // 发送更新消息
            _messenger.Send(new CreatorUpdatedMessage());
            
            // 关闭对话框（通过 Window 的 DialogResult）
            RequestClose?.Invoke(this, true);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            HasError = true;
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        RequestClose?.Invoke(this, false);
    }

    public event EventHandler<bool>? RequestClose;
}
