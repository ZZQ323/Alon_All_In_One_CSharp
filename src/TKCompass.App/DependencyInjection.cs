using Microsoft.Extensions.DependencyInjection;
using TKCompass.Data;
using TKCompass.Data.Repositories;
using TKCompass.Domain.Repositories;
using TKCompass.Domain.Services;
using TKCompass.Excel;
using Microsoft.EntityFrameworkCore;

namespace TKCompass;

public static class DependencyInjection
{
    public static IServiceCollection AddTKCompassServices(this IServiceCollection services, string dbPath)
    {
        // 注册 DbContext
        services.AddDbContext<TKCompassDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));

        // 注册 Repositories
        services.AddScoped<ICreatorRepository, CreatorRepository>();
        services.AddScoped<IShipmentRepository, ShipmentRepository>();
        services.AddScoped<IVideoRepository, VideoRepository>();
        services.AddScoped<ICreatorContactRepository, CreatorContactRepository>();
        services.AddScoped<IStatusFlowHistoryRepository, StatusFlowHistoryRepository>();
        services.AddScoped<IOperationLogRepository, OperationLogRepository>();
        services.AddScoped<IDictionaryRepository, DictionaryRepository>();
        services.AddScoped<ICreatorTypeRepository, CreatorTypeRepository>();
        services.AddScoped<IProductCodeRepository, ProductCodeRepository>();
        services.AddScoped<ITagDefinitionRepository, TagDefinitionRepository>();
        services.AddScoped<ICreatorTagRepository, CreatorTagRepository>();
        services.AddScoped<IRelationshipRepository, RelationshipRepository>();
        services.AddScoped<IBatchRemindLogRepository, BatchRemindLogRepository>();
        services.AddScoped<IFulfillmentRepository, FulfillmentRepository>();

        // 注册 Services
        services.AddScoped<ICreatorService, CreatorService>();
        services.AddScoped<IShipmentService, ShipmentService>();
        services.AddScoped<IVideoService, VideoService>();
        services.AddScoped<ICreatorContactService, CreatorContactService>();
        services.AddScoped<IStatusFlowHistoryService, StatusFlowHistoryService>();
        services.AddScoped<IOperationLogService, OperationLogService>();
        services.AddScoped<IDictionaryService, DictionaryService>();
        services.AddScoped<ICreatorTypeService, CreatorTypeService>();
        services.AddScoped<IProductCodeService, ProductCodeService>();
        services.AddScoped<IBatchRemindLogService, BatchRemindLogService>();
        services.AddScoped<IRelationshipService, RelationshipService>();
        services.AddScoped<IUndoService, UndoService>();
        services.AddScoped<ExcelImportService>();
        services.AddScoped<IFollowUpService, FollowUpService>();
        services.AddScoped<IBackupService, BackupService>();
        services.AddScoped<IFulfillmentService, FulfillmentService>();
        services.AddScoped<ITodayTaskService, TodayTaskService>();

        return services;
    }
}
