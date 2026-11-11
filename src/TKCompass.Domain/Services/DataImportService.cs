using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text.Json;
using System.Threading.Tasks;
using TKCompass.Domain.Models;
using TKCompass.Domain.Repositories;

namespace TKCompass.Domain.Services;

public interface IDataImportService
{
    Task ImportDataAsync(string zipPath);
}

public class DataImportService : IDataImportService
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
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public DataImportService(
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

    public async Task ImportDataAsync(string zipPath)
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"tkcompass_import_{Guid.NewGuid()}");
        
        try
        {
            // 解压 ZIP 文件
            ZipFile.ExtractToDirectory(zipPath, tempDir);
            
            // 清空现有数据（按依赖顺序）
            await ClearAllDataAsync();
            
            // 按依赖顺序导入
            await ImportCreatorTypesAsync(tempDir);
            await ImportProductCodesAsync(tempDir);
            await ImportDictionariesAsync(tempDir);
            await ImportTagsAsync(tempDir);
            await ImportCreatorsAsync(tempDir);
            await ImportShipmentsAsync(tempDir);
            await ImportVideosAsync(tempDir);
            await ImportContactsAsync(tempDir);
            await ImportHistoriesAsync(tempDir);
            await ImportRelationshipsAsync(tempDir);
            await ImportOperationLogsAsync(tempDir);
            await ImportBatchRemindLogsAsync(tempDir);
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

    private async Task ClearAllDataAsync()
    {
        // 按依赖顺序清空
        var operationLogs = await _operationLogRepository.GetAllAsync();
        foreach (var log in operationLogs)
            await _operationLogRepository.DeleteAsync(log);
        
        var batchLogs = await _batchRemindLogRepository.GetAllAsync();
        foreach (var log in batchLogs)
            await _batchRemindLogRepository.DeleteAsync(log);
        
        var histories = await _historyRepository.GetAllAsync();
        foreach (var h in histories)
            await _historyRepository.DeleteAsync(h);
        
        var contacts = await _contactRepository.GetAllAsync();
        foreach (var c in contacts)
            await _contactRepository.DeleteAsync(c);
        
        var videos = await _videoRepository.GetAllAsync();
        foreach (var v in videos)
            await _videoRepository.PermanentDeleteAsync(v.Id);
        
        var shipments = await _shipmentRepository.GetAllAsync();
        foreach (var s in shipments)
            await _shipmentRepository.PermanentDeleteAsync(s.Id);
        
        var relationships = await _relationshipRepository.GetAllAsync();
        foreach (var r in relationships)
            await _relationshipRepository.DeleteAsync(r);
        
        var creators = await _creatorRepository.GetAllIncludeDeletedAsync();
        foreach (var c in creators)
            await _creatorRepository.PermanentDeleteAsync(c.Id);
        
        var creatorTypes = await _creatorTypeRepository.GetAllAsync();
        foreach (var ct in creatorTypes)
            await _creatorTypeRepository.PermanentDeleteAsync(ct.Id);
        
        var productCodes = await _productCodeRepository.GetAllAsync();
        foreach (var pc in productCodes)
            await _productCodeRepository.PermanentDeleteAsync(pc.Id);
        
        var dictionaries = await _dictionaryRepository.GetAllAsync();
        foreach (var d in dictionaries)
            await _dictionaryRepository.DeleteAsync(d);
        
        var tags = await _tagRepository.GetAllAsync();
        foreach (var t in tags)
            await _tagRepository.PermanentDeleteAsync(t.Id);
        
        await _operationLogRepository.SaveChangesAsync();
        await _batchRemindLogRepository.SaveChangesAsync();
        await _historyRepository.SaveChangesAsync();
        await _contactRepository.SaveChangesAsync();
        await _videoRepository.SaveChangesAsync();
        await _shipmentRepository.SaveChangesAsync();
        await _relationshipRepository.SaveChangesAsync();
        await _creatorRepository.SaveChangesAsync();
        await _creatorTypeRepository.SaveChangesAsync();
        await _productCodeRepository.SaveChangesAsync();
        await _dictionaryRepository.SaveChangesAsync();
        await _tagRepository.SaveChangesAsync();
    }

    private async Task ImportCreatorTypesAsync(string tempDir)
    {
        var filePath = Path.Combine(tempDir, "creator_types.json");
        if (!File.Exists(filePath)) return;
        
        var json = await File.ReadAllTextAsync(filePath);
        var items = JsonSerializer.Deserialize<List<CreatorType>>(json, JsonOptions);
        if (items == null) return;
        
        foreach (var item in items)
        {
            await _creatorTypeRepository.AddAsync(item);
        }
        await _creatorTypeRepository.SaveChangesAsync();
    }

    private async Task ImportProductCodesAsync(string tempDir)
    {
        var filePath = Path.Combine(tempDir, "product_codes.json");
        if (!File.Exists(filePath)) return;
        
        var json = await File.ReadAllTextAsync(filePath);
        var items = JsonSerializer.Deserialize<List<ProductCode>>(json, JsonOptions);
        if (items == null) return;
        
        foreach (var item in items)
        {
            await _productCodeRepository.AddAsync(item);
        }
        await _productCodeRepository.SaveChangesAsync();
    }

    private async Task ImportDictionariesAsync(string tempDir)
    {
        var filePath = Path.Combine(tempDir, "dictionaries.json");
        if (!File.Exists(filePath)) return;
        
        var json = await File.ReadAllTextAsync(filePath);
        var items = JsonSerializer.Deserialize<List<Dictionary>>(json, JsonOptions);
        if (items == null) return;
        
        foreach (var item in items)
        {
            await _dictionaryRepository.AddAsync(item);
        }
        await _dictionaryRepository.SaveChangesAsync();
    }

    private async Task ImportTagsAsync(string tempDir)
    {
        var filePath = Path.Combine(tempDir, "tags.json");
        if (!File.Exists(filePath)) return;
        
        var json = await File.ReadAllTextAsync(filePath);
        var items = JsonSerializer.Deserialize<List<TagDefinition>>(json, JsonOptions);
        if (items == null) return;
        
        foreach (var item in items)
        {
            await _tagRepository.AddAsync(item);
        }
        await _tagRepository.SaveChangesAsync();
    }

    private async Task ImportCreatorsAsync(string tempDir)
    {
        var filePath = Path.Combine(tempDir, "creators.json");
        if (!File.Exists(filePath)) return;
        
        var json = await File.ReadAllTextAsync(filePath);
        var items = JsonSerializer.Deserialize<List<Creator>>(json, JsonOptions);
        if (items == null) return;
        
        foreach (var item in items)
        {
            await _creatorRepository.AddAsync(item);
        }
        await _creatorRepository.SaveChangesAsync();
    }

    private async Task ImportShipmentsAsync(string tempDir)
    {
        var filePath = Path.Combine(tempDir, "shipments.json");
        if (!File.Exists(filePath)) return;
        
        var json = await File.ReadAllTextAsync(filePath);
        var items = JsonSerializer.Deserialize<List<Shipment>>(json, JsonOptions);
        if (items == null) return;
        
        foreach (var item in items)
        {
            await _shipmentRepository.AddAsync(item);
        }
        await _shipmentRepository.SaveChangesAsync();
    }

    private async Task ImportVideosAsync(string tempDir)
    {
        var filePath = Path.Combine(tempDir, "videos.json");
        if (!File.Exists(filePath)) return;
        
        var json = await File.ReadAllTextAsync(filePath);
        var items = JsonSerializer.Deserialize<List<Video>>(json, JsonOptions);
        if (items == null) return;
        
        foreach (var item in items)
        {
            await _videoRepository.AddAsync(item);
        }
        await _videoRepository.SaveChangesAsync();
    }

    private async Task ImportContactsAsync(string tempDir)
    {
        var filePath = Path.Combine(tempDir, "creator_contacts.json");
        if (!File.Exists(filePath)) return;
        
        var json = await File.ReadAllTextAsync(filePath);
        var items = JsonSerializer.Deserialize<List<CreatorContact>>(json, JsonOptions);
        if (items == null) return;
        
        foreach (var item in items)
        {
            await _contactRepository.AddAsync(item);
        }
        await _contactRepository.SaveChangesAsync();
    }

    private async Task ImportHistoriesAsync(string tempDir)
    {
        var filePath = Path.Combine(tempDir, "status_flow_history.json");
        if (!File.Exists(filePath)) return;
        
        var json = await File.ReadAllTextAsync(filePath);
        var items = JsonSerializer.Deserialize<List<StatusFlowHistory>>(json, JsonOptions);
        if (items == null) return;
        
        foreach (var item in items)
        {
            await _historyRepository.AddAsync(item);
        }
        await _historyRepository.SaveChangesAsync();
    }

    private async Task ImportRelationshipsAsync(string tempDir)
    {
        var filePath = Path.Combine(tempDir, "relationships.json");
        if (!File.Exists(filePath)) return;
        
        var json = await File.ReadAllTextAsync(filePath);
        var items = JsonSerializer.Deserialize<List<Relationship>>(json, JsonOptions);
        if (items == null) return;
        
        foreach (var item in items)
        {
            await _relationshipRepository.AddAsync(item);
        }
        await _relationshipRepository.SaveChangesAsync();
    }

    private async Task ImportOperationLogsAsync(string tempDir)
    {
        var filePath = Path.Combine(tempDir, "operation_log.json");
        if (!File.Exists(filePath)) return;
        
        var json = await File.ReadAllTextAsync(filePath);
        var items = JsonSerializer.Deserialize<List<OperationLog>>(json, JsonOptions);
        if (items == null) return;
        
        foreach (var item in items)
        {
            await _operationLogRepository.AddAsync(item);
        }
        await _operationLogRepository.SaveChangesAsync();
    }

    private async Task ImportBatchRemindLogsAsync(string tempDir)
    {
        var filePath = Path.Combine(tempDir, "batch_remind_log.json");
        if (!File.Exists(filePath)) return;
        
        var json = await File.ReadAllTextAsync(filePath);
        var items = JsonSerializer.Deserialize<List<BatchRemindLog>>(json, JsonOptions);
        if (items == null) return;
        
        foreach (var item in items)
        {
            await _batchRemindLogRepository.AddAsync(item);
        }
        await _batchRemindLogRepository.SaveChangesAsync();
    }
}
