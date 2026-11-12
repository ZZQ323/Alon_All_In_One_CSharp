using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TKCompass.Domain.Services;
using TKCompass.Excel;
using TKCompass.Services;

namespace TKCompass.ViewModels;

public partial class MainViewModel : ObservableObject, IRecipient<OpenCreatorDetailMessage>, IRecipient<OpenCreateCreatorDialogMessage>
{
    private readonly ICreatorService _creatorService;
    private readonly ICreatorTypeService _creatorTypeService;
    private readonly IProductCodeService _productCodeService;
    private readonly IDictionaryService _dictionaryService;
    private readonly IOperationLogService _operationLogService;
    private readonly IShipmentService _shipmentService;
    private readonly IVideoService _videoService;
    private readonly IRelationshipService _relationshipService;
    private readonly ICreatorContactService _contactService;
    private readonly ExcelImportService _excelImportService;
    private readonly IMessenger _messenger;
    private readonly INotificationService? _notificationService;
    private readonly IBackupService _backupService;

    [ObservableProperty]
    private string _statusBarMessage = "就绪";

    [ObservableProperty]
    private CreatorDashboardViewModel _dashboard;

    [ObservableProperty]
    private ConfigViewModel _config;

    [ObservableProperty]
    private SettingsViewModel _settings;

    [ObservableProperty]
    private OperationHistoryViewModel _operationHistory;

    [ObservableProperty]
    private ExcelImportViewModel _excelImport;

    [ObservableProperty]
    private CrmViewModel _crm;

    [ObservableProperty]
    private TodayReminderViewModel _todayReminder;

    [ObservableProperty]
    private InDeliveryViewModel _inDelivery;

    [ObservableProperty]
    private AllCreatorsViewModel _allCreators;

    [ObservableProperty]
    private DeliveredViewModel _delivered;

    [ObservableProperty]
    private DeletedViewModel _deleted;

    [ObservableProperty]
    private RemindedViewModel _reminded;

    [ObservableProperty]
    private string _selectedNavigation = "Dashboard";

    [ObservableProperty]
    private bool _isDashboardExpanded;

    [ObservableProperty]
    private object _currentView;

    public MainViewModel(
        ICreatorService creatorService,
        ICreatorTypeService creatorTypeService,
        IProductCodeService productCodeService,
        IDictionaryService dictionaryService,
        IOperationLogService operationLogService,
        IShipmentService shipmentService,
        IVideoService videoService,
        IRelationshipService relationshipService,
        ICreatorContactService contactService,
        ExcelImportService excelImportService,
        IUndoService undoService,
        IMessenger messenger,
        IBackupService backupService,
        INotificationService? notificationService = null)
    {
        _creatorService = creatorService;
        _creatorTypeService = creatorTypeService;
        _productCodeService = productCodeService;
        _dictionaryService = dictionaryService;
        _operationLogService = operationLogService;
        _shipmentService = shipmentService;
        _videoService = videoService;
        _relationshipService = relationshipService;
        _contactService = contactService;
        _excelImportService = excelImportService;
        _messenger = messenger;
        _notificationService = notificationService;
        _backupService = backupService;

        _dashboard = new CreatorDashboardViewModel(creatorService, messenger);
        _config = new ConfigViewModel(creatorTypeService, productCodeService, dictionaryService, messenger);
        _settings = new SettingsViewModel(operationLogService, backupService);
        _operationHistory = new OperationHistoryViewModel(operationLogService, undoService, messenger);
        _excelImport = new ExcelImportViewModel(
            excelImportService, 
            creatorService, 
            shipmentService, 
            videoService, 
            contactService, 
            operationLogService,
            messenger);
        _crm = new CrmViewModel(creatorService, messenger);
        _todayReminder = new TodayReminderViewModel();
        _inDelivery = new InDeliveryViewModel();
        _allCreators = new AllCreatorsViewModel(creatorService, messenger);
        _delivered = new DeliveredViewModel();
        _deleted = new DeletedViewModel();
        _reminded = new RemindedViewModel();
        _currentView = _dashboard;
    }

    public void SetStatus(string message)
    {
        StatusBarMessage = message;
        // 5秒后自动恢复
        _ = Task.Delay(5000).ContinueWith(_ =>
            Dispatcher.UIThread.Post(() => StatusBarMessage = "就绪"));
    }

    [RelayCommand]
    private void NavigateToDashboard()
    {
        CurrentView = Dashboard;
        SelectedNavigation = "Dashboard";
        SetStatus("已切换到 达人工作台 页面");
    }

    [RelayCommand]
    private void NavigateToConfig()
    {
        CurrentView = Config;
        SelectedNavigation = "Config";
        SetStatus("已切换到 配置管理 页面");
    }

    [RelayCommand]
    private void NavigateToSettings()
    {
        CurrentView = Settings;
        SelectedNavigation = "Settings";
        SetStatus("已切换到 设置 页面");
    }

    [RelayCommand]
    private void NavigateToOperationHistory()
    {
        CurrentView = OperationHistory;
        SelectedNavigation = "OperationHistory";
        SetStatus("已切换到 操作历史 页面");
    }

    [RelayCommand]
    private void NavigateToExcelImport()
    {
        CurrentView = ExcelImport;
        SelectedNavigation = "ExcelImport";
        SetStatus("已切换到 Excel 导入 页面");
    }

    [RelayCommand]
    private void NavigateToCrm()
    {
        CurrentView = Crm;
        SelectedNavigation = "Crm";
        SetStatus("已切换到 CRM 管理 页面");
    }

    [RelayCommand]
    private void NavigateToTodayReminder()
    {
        CurrentView = TodayReminder;
        SelectedNavigation = "TodayReminder";
        SetStatus("已切换到 今日提醒 页面");
    }

    [RelayCommand]
    private void NavigateToInDelivery()
    {
        CurrentView = InDelivery;
        SelectedNavigation = "InDelivery";
        SetStatus("已切换到 正在交付 页面");
    }

    [RelayCommand]
    private void NavigateToAllCreators()
    {
        CurrentView = AllCreators;
        SelectedNavigation = "AllCreators";
        SetStatus("已切换到 总表 页面");
    }

    [RelayCommand]
    private void NavigateToDelivered()
    {
        CurrentView = Delivered;
        SelectedNavigation = "Delivered";
        SetStatus("已切换到 已交付 页面");
    }

    [RelayCommand]
    private void NavigateToDeleted()
    {
        CurrentView = Deleted;
        SelectedNavigation = "Deleted";
        SetStatus("已切换到 已删除 页面");
    }

    [RelayCommand]
    private void NavigateToReminded()
    {
        CurrentView = Reminded;
        SelectedNavigation = "Reminded";
        SetStatus("已切换到 已提醒 页面");
    }

    [RelayCommand]
    private void Refresh()
    {
        // 触发当前视图刷新 - 通过重新赋值 CurrentView 来触发 UI 刷新
        var current = CurrentView;
        CurrentView = null!;
        CurrentView = current;
        SetStatus("已刷新当前页面");
    }

    public async void Receive(OpenCreatorDetailMessage message)
    {
        var detailVm = new CreatorDetailViewModel(_creatorService, _shipmentService, _videoService, _contactService, _productCodeService, _messenger);
        await detailVm.LoadCreatorAsync(message.CreatorId);
        CurrentView = detailVm;
        SetStatus("已打开达人详情");
    }

    public void Receive(OpenCreateCreatorDialogMessage message)
    {
        // 打开新建达人弹窗
        var dialog = new Views.CreateCreatorDialog();
        var viewModel = new CreateCreatorDialogViewModel(_creatorService, _messenger);
        dialog.DataContext = viewModel;
        
        // 获取主窗口并显示对话框
        if (Avalonia.Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime lifetime)
        {
            dialog.ShowDialog(lifetime.MainWindow);
        }
    }
}
