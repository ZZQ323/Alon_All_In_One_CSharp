using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TKCompass.Domain.Models;
using TKCompass.Domain.Services;
using TKCompass.Excel;
using TKCompass.Services;

namespace TKCompass.ViewModels;

public partial class ExcelImportViewModel : ObservableObject
{
    private readonly ExcelImportService _excelImportService;
    private readonly ICreatorService _creatorService;
    private readonly IShipmentService _shipmentService;
    private readonly IVideoService _videoService;
    private readonly ICreatorContactService _contactService;
    private readonly IOperationLogService _operationLogService;
    private readonly IMessenger _messenger;
    private readonly INotificationService? _notificationService;

    [ObservableProperty]
    private string _filePath = string.Empty;

    [ObservableProperty]
    private ImportPreview? _preview;

    [ObservableProperty]
    private bool _isImporting;

    // Alias for standardized loading overlay binding
    public bool IsLoading => IsImporting;

    partial void OnIsImportingChanged(bool value)
    {
        OnPropertyChanged(nameof(IsLoading));
    }

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private bool _hasErrors;

    public ExcelImportViewModel(
        ExcelImportService excelImportService,
        ICreatorService creatorService,
        IShipmentService shipmentService,
        IVideoService videoService,
        ICreatorContactService contactService,
        IOperationLogService operationLogService,
        IMessenger messenger,
        INotificationService? notificationService = null)
    {
        _excelImportService = excelImportService;
        _creatorService = creatorService;
        _shipmentService = shipmentService;
        _videoService = videoService;
        _contactService = contactService;
        _operationLogService = operationLogService;
        _messenger = messenger;
        _notificationService = notificationService;
    }

    [RelayCommand]
    private async Task SelectFileAsync()
    {
        // 在实际应用中，这里会打开文件选择对话框
        // 由于 Avalonia 的文件对话框需要在 Window 中调用，这里简化处理
        StatusMessage = "请选择 Excel 文件（实际应用中会打开文件选择对话框）";
        HasErrors = true;
    }

    [RelayCommand]
    private async Task ParseFileAsync()
    {
        if (string.IsNullOrWhiteSpace(FilePath))
        {
            StatusMessage = "请先选择文件";
            HasErrors = true;
            return;
        }

        try
        {
            StatusMessage = "正在解析文件...";
            HasErrors = false;

            Preview = await Task.Run(() => _excelImportService.ParseExcel(FilePath));

            if (Preview.Errors.Count > 0)
            {
                StatusMessage = $"解析完成，发现 {Preview.Errors.Count} 个错误";
                HasErrors = true;
            }
            else
            {
                StatusMessage = $"解析成功：{Preview.CreatorRows.Count} 个达人，{Preview.VideoRows.Count} 个视频";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"解析失败：{ex.Message}";
            HasErrors = true;
            Preview = null;
        }
    }

    [RelayCommand]
    private async Task ImportAsync()
    {
        if (Preview == null)
        {
            StatusMessage = "请先解析文件";
            HasErrors = true;
            return;
        }

        if (Preview.ErrorCount > 0)
        {
            StatusMessage = $"存在错误，无法导入";
            HasErrors = true;
            return;
        }

        try
        {
            IsImporting = true;
            StatusMessage = "正在导入...";
            HasErrors = false;

            int creatorCount = 0;
            int shipmentCount = 0;
            int videoCount = 0;

            // 1. 导入达人主档
            foreach (var row in Preview.CreatorRows.Where(r => r.IsValid))
            {
                // 检查达人是否已存在
                var existingCreator = await _creatorService.GetByHandleAsync(row.Handle);
                if (existingCreator != null)
                {
                    // 跳过已存在的达人
                    continue;
                }

                // 创建达人
                var creator = await _creatorService.CreateCreatorAsync(
                    handle: row.Handle,
                    nickname: row.Handle, // 使用 handle 作为昵称
                    qualityLevel: row.QualityLevel
                );
                creatorCount++;

                // 创建寄样记录
                if (!string.IsNullOrWhiteSpace(row.TrackingNumber))
                {
                    var shipment = await _shipmentService.CreateShipmentAsync(
                        creatorId: creator.Id,
                        trackingNumber: row.TrackingNumber,
                        sampleDate: row.SampleDate
                    );
                    shipmentCount++;

                    // 创建视频记录
                    if (!string.IsNullOrWhiteSpace(row.VideoUrl))
                    {
                        var video = await _videoService.CreateVideoAsync(
                            creatorId: creator.Id,
                            videoUrl: row.VideoUrl
                        );
                        videoCount++;
                    }
                }

                // 创建联系方式
                if (!string.IsNullOrWhiteSpace(row.Whatsapp))
                {
                    await _contactService.AddContactAsync(
                        creatorId: creator.Id,
                        type: Domain.Enums.ContactType.WhatsApp,
                        value: row.Whatsapp
                    );
                }

                if (!string.IsNullOrWhiteSpace(row.Email))
                {
                    await _contactService.AddContactAsync(
                        creatorId: creator.Id,
                        type: Domain.Enums.ContactType.Email,
                        value: row.Email
                    );
                }

                if (!string.IsNullOrWhiteSpace(row.OtherContact))
                {
                    await _contactService.AddContactAsync(
                        creatorId: creator.Id,
                        type: Domain.Enums.ContactType.Other,
                        value: row.OtherContact
                    );
                }

                // 更新最近联系日期
                if (row.LatestContactDate.HasValue)
                {
                    creator.LatestContactDate = row.LatestContactDate.Value;
                    await _creatorService.UpdateAsync(creator);
                }
            }

            // 2. 导入视频记录
            foreach (var row in Preview.VideoRows.Where(r => r.IsValid))
            {
                // 查找达人
                var creator = await _creatorService.GetByHandleAsync(row.Handle);
                if (creator == null)
                {
                    // 跳过未找到达人的视频
                    continue;
                }

                // 创建视频
                var video = await _videoService.CreateVideoAsync(
                    creatorId: creator.Id,
                    videoUrl: row.VideoUrl
                );
                videoCount++;

                // 更新广告代码
                if (!string.IsNullOrWhiteSpace(row.AdCode))
                {
                    video.AdCode = row.AdCode;
                    await _videoService.UpdateAsync(video);
                }
            }

            StatusMessage = $"导入成功：{creatorCount} 个达人，{shipmentCount} 个寄样，{videoCount} 个视频";
            _messenger.Send(new CreatorUpdatedMessage());
            
            // 显示成功通知
            if (_notificationService != null)
            {
                await _notificationService.ShowInfoAsync(
                    "导入成功", 
                    $"成功导入 {creatorCount} 个达人，{shipmentCount} 个寄样，{videoCount} 个视频");
            }
            
            // 清空预览
            Preview = null;
            FilePath = string.Empty;
        }
        catch (Exception ex)
        {
            StatusMessage = $"导入失败：{ex.Message}";
            HasErrors = true;
            if (_notificationService != null)
                await _notificationService.ShowErrorAsync("导入失败", ex.Message);
        }
        finally
        {
            IsImporting = false;
        }
    }
}
