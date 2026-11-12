using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using TKCompass.Domain.Services;

namespace TKCompass.ViewModels;

/// <summary>
/// 新建达人弹窗 ViewModel
/// </summary>
public partial class CreateCreatorDialogViewModel : ObservableObject
{
    private readonly ICreatorService? _creatorService;
    private readonly IMessenger? _messenger;

    [ObservableProperty]
    private string _nickname = string.Empty;

    [ObservableProperty]
    private string _handle = string.Empty;

    [ObservableProperty]
    private string _contactInfo = string.Empty;

    [ObservableProperty]
    private string _qualityLevel = "C";

    [ObservableProperty]
    private string? _notes;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _hasError;

    public CreateCreatorDialogViewModel()
    {
    }

    public CreateCreatorDialogViewModel(ICreatorService creatorService, IMessenger messenger)
    {
        _creatorService = creatorService;
        _messenger = messenger;
    }

    public async Task<bool> SubmitAsync()
    {
        HasError = false;
        ErrorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(Nickname))
        {
            ErrorMessage = "昵称不能为空";
            HasError = true;
            return false;
        }

        if (string.IsNullOrWhiteSpace(Handle))
        {
            ErrorMessage = "平台账号不能为空";
            HasError = true;
            return false;
        }

        if (string.IsNullOrWhiteSpace(ContactInfo))
        {
            ErrorMessage = "联系方式不能为空";
            HasError = true;
            return false;
        }

        if (_creatorService == null)
            return false;

        try
        {
            var creator = await _creatorService.CreateCreatorAsync(Handle.Trim(), Nickname.Trim(), QualityLevel.Trim());
            _messenger?.Send(new CreatorUpdatedMessage());
            return true;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"创建失败：{ex.Message}";
            HasError = true;
            return false;
        }
    }
}
