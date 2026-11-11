using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text.Json;
using System.Threading.Tasks;
using TKCompass.Domain.Models;
using TKCompass.Domain.Repositories;

namespace TKCompass.Domain.Services;

public interface IDataExportService
{
    Task<string> ExportDataAsync(string outputPath);
}

public class DataExportService : IDataExportService
{
    private readonly ICreatorRepository _creatorRepository;
    private readonly IShipmentRepository _shipmentRepository;
    private readonly IVideoRepository _videoRepository;
    private readonly ICreatorContactRepository _contactRepository;
    private readonly IStatusFlowHistoryRepository _historyRepository;
    private readonly ICreatorTypeRepository _creatorTypeRepository;
    private readonly IProductCodeRepository _productCodeRepository;
    private readonly IDictionaryRepository _dictionaryRepository;
    private readonly ITagDefinitionRepository _tagRepository;
    private readonly IRelationshipRepository _relationshipRepository;
    private readonly IOperationLogRepository _operationLogRepository;
    private readonly IBatchRemindLogRepository _batchRemindLogRepository;
    
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public DataExportService(
        ICreatorRepository creatorRepository,
        IShipmentRepository shipmentRepository,
        IVideoRepository videoRepository,
        ICreatorContactRepository contactRepository,
        IStatusFlowHistoryRepository historyRepository,
        ICreatorTypeRepository creatorTypeRepository,
        IProductCodeRepository productCodeRepository,
        IDictionaryRepository dictionaryRepository,
        ITagDefinitionRepository tagRepository,
        IRelationshipRepository relationshipRepository,
        IOperationLogRepository operationLogRepository,
        IBatchRemindLogRepository batchRemindLogRepository)
    {
        _creatorRepository = creatorRepository;
        _shipmentRepository = shipmentRepository;
        _videoRepository = videoRepository;
        _contactRepository = contactRepository;
        _historyRepository = historyRepository;
        _creatorTypeRepository = creatorTypeRepository;
        _productCodeRepository = productCodeRepository;
        _dictionaryRepository = dictionaryRepository;
        _tagRepository = tagRepository;
        _relationshipRepository = relationshipRepository;
        _operationLogRepository = operationLogRepository;
        _batchRemindLogRepository = batchRemindLogRepository;
    }

    public async Task<string> ExportDataAsync(string outputPath)
    {
        // 收集所有数据
        var data = new Dictionary<string, object>
        {
            ["metadata"] = new
            {
                exportVersion = "1.0",
                exportTime = DateTime.Now.ToString("o"),
                appVersion = "1.0.0"
            },
            ["creators"] = await _creatorRepository.GetAllIncludeDeletedAsync(),
            ["shipments"] = await _shipmentRepository.GetAllAsync(),
            ["videos"] = await _videoRepository.GetAllAsync(),
            ["creator_contacts"] = await _contactRepository.GetAllAsync(),
            ["status_flow_history"] = await _historyRepository.GetAllAsync(),
            ["creator_types"] = await _creatorTypeRepository.GetAllAsync(),
            ["product_codes"] = await _productCodeRepository.GetAllAsync(),
            ["dictionaries"] = await _dictionaryRepository.GetAllAsync(),
            ["tags"] = await _tagRepository.GetAllAsync(),
            ["relationships"] = await _relationshipRepository.GetAllAsync(),
            ["operation_log"] = await _operationLogRepository.GetAllAsync(),
            ["batch_remind_log"] = await _batchRemindLogRepository.GetAllAsync()
        };

        // 创建临时 JSON 文件
        var tempDir = Path.Combine(Path.GetTempPath(), $"tkcompass_export_{Guid.NewGuid()}");
        Directory.CreateDirectory(tempDir);

        try
        {
            // 写入各个 JSON 文件
            foreach (var kvp in data)
            {
                var json = JsonSerializer.Serialize(kvp.Value, JsonOptions);
                var filePath = Path.Combine(tempDir, $"{kvp.Key}.json");
                await File.WriteAllTextAsync(filePath, json);
            }

            // 创建 ZIP 文件
            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }
            ZipFile.CreateFromDirectory(tempDir, outputPath);

            return outputPath;
        }
        finally
        {
            // 清理临时目录
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }
}
