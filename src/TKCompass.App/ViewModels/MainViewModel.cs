using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TKCompass.Domain.Services;
using TKCompass.Excel;

namespace TKCompass.ViewModels;

public partial class MainViewModel : ObservableObject
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
    private TodayFollowUpViewModel _todayFollowUp;

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
        IMessenger messenger)
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

        _dashboard = new CreatorDashboardViewModel(creatorService, messenger);
        _config = new ConfigViewModel(creatorTypeService, productCodeService, dictionaryService, messenger);
        _settings = new SettingsViewModel(operationLogService);
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
        _todayFollowUp = new TodayFollowUpViewModel(
            creatorService, shipmentService, videoService, 
            relationshipService, contactService, productCodeService, dictionaryService, messenger);
        _currentView = _dashboard;
    }

    [RelayCommand]
    private void NavigateToDashboard()
    {
        CurrentView = Dashboard;
    }

    [RelayCommand]
    private void NavigateToConfig()
    {
        CurrentView = Config;
    }

    [RelayCommand]
    private void NavigateToSettings()
    {
        CurrentView = Settings;
    }

    [RelayCommand]
    private void NavigateToOperationHistory()
    {
        CurrentView = OperationHistory;
    }

    [RelayCommand]
    private void NavigateToExcelImport()
    {
        CurrentView = ExcelImport;
    }

    [RelayCommand]
    private void NavigateToCrm()
    {
        CurrentView = Crm;
    }

    [RelayCommand]
    private void NavigateToTodayFollowUp()
    {
        CurrentView = TodayFollowUp;
    }
}
