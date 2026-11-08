using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TKCompass.Data;
using TKCompass.ViewModels;
using TKCompass.Views;

namespace TKCompass;

public partial class App : Application
{
    public static IServiceProvider? Services { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // 配置依赖注入
        var services = new ServiceCollection();
        
        // 获取数据库路径（使用 %APPDATA%/TKCompass/tkcompass.db）
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var dbFolder = Path.Combine(appDataPath, "TKCompass");
        
        // 确保目录存在
        if (!Directory.Exists(dbFolder))
        {
            Directory.CreateDirectory(dbFolder);
        }
        
        var dbPath = Path.Combine(dbFolder, "tkcompass.db");
        
        // 注册服务
        services.AddTKCompassServices(dbPath);
        
        // 注册 Messenger
        services.AddSingleton<IMessenger, WeakReferenceMessenger>();
        
        // 注册 ViewModels
        services.AddTransient<MainViewModel>();
        services.AddTransient<CreatorDashboardViewModel>();
        services.AddTransient<CreatorEditViewModel>();
        services.AddTransient<ConfigViewModel>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<OperationHistoryViewModel>();
        services.AddTransient<ExcelImportViewModel>();
        services.AddTransient<CrmViewModel>();
        services.AddTransient<ShipmentDetailViewModel>();
        services.AddTransient<ProductMultiSelectViewModel>();
        services.AddTransient<CreatorTypeMultiSelectViewModel>();
        services.AddTransient<BatchRemindLogViewModel>();
        services.AddTransient<TodayFollowUpViewModel>();
        
        Services = services.BuildServiceProvider();
        
        // 确保数据库已创建
        using (var scope = Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<TKCompassDbContext>();
            dbContext.Database.EnsureCreated();
        }
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = Services.GetRequiredService<MainViewModel>(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
