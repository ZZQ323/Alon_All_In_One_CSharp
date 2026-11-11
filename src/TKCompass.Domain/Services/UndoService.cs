using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using TKCompass.Domain.Models;
using TKCompass.Domain.Repositories;

namespace TKCompass.Domain.Services;

public interface IUndoService
{
    Task<bool> UndoLastAsync();
    Task<bool> UndoGroupAsync(string groupId);
    Task<IEnumerable<OperationLog>> GetRecentOperationsAsync(int count = 20);
}

public class UndoService : IUndoService
{
    private readonly IOperationLogRepository _logRepository;
    private readonly ICreatorRepository _creatorRepository;
    private readonly IShipmentRepository _shipmentRepository;
    private readonly IVideoRepository _videoRepository;
    
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = false
    };

    public UndoService(
        IOperationLogRepository logRepository,
        ICreatorRepository creatorRepository,
        IShipmentRepository shipmentRepository,
        IVideoRepository videoRepository)
    {
        _logRepository = logRepository;
        _creatorRepository = creatorRepository;
        _shipmentRepository = shipmentRepository;
        _videoRepository = videoRepository;
    }

    public async Task<bool> UndoLastAsync()
    {
        var recentOps = await _logRepository.GetRecentAsync(20);
        
        // 找到最近的可撤销操作（跳过 UNDO 类型）
        OperationLog? lastOp = null;
        foreach (var op in recentOps)
        {
            if (op.OperationType != "UNDO" && 
                (op.OperationType == "CREATE" || op.OperationType == "UPDATE" || op.OperationType == "DELETE"))
            {
                lastOp = op;
                break;
            }
        }
        
        if (lastOp == null) return false;
        
        // 如果有 group_id，撤销整个组
        if (!string.IsNullOrEmpty(lastOp.GroupId))
        {
            return await UndoGroupAsync(lastOp.GroupId);
        }
        
        return await UndoSingleAsync(lastOp);
    }

    public async Task<bool> UndoGroupAsync(string groupId)
    {
        var groupOps = await _logRepository.GetByGroupIdAsync(groupId);
        bool success = true;
        
        // 逆序撤销
        var ops = new List<OperationLog>(groupOps);
        ops.Reverse();
        
        foreach (var op in ops)
        {
            if (!await UndoSingleAsync(op))
            {
                success = false;
            }
        }
        
        return success;
    }

    private async Task<bool> UndoSingleAsync(OperationLog op)
    {
        try
        {
            switch (op.EntityType)
            {
                case "Creator":
                    await UndoCreatorAsync(op);
                    break;
                case "Shipment":
                    await UndoShipmentAsync(op);
                    break;
                case "Video":
                    await UndoVideoAsync(op);
                    break;
                default:
                    return false;
            }
            
            // 记录撤销操作
            op.Undone = true;
            await _logRepository.UpdateAsync(op);
            await _logRepository.SaveChangesAsync();
            
            return true;
        }
        catch
        {
            return false;
        }
    }

    private async Task UndoCreatorAsync(OperationLog op)
    {
        if (op.OperationType == "CREATE")
        {
            // 撤销创建 = 删除
            if (long.TryParse(op.EntityId, out var id))
            {
                await _creatorRepository.PermanentDeleteAsync(id);
                await _creatorRepository.SaveChangesAsync();
            }
        }
        else if (op.OperationType == "DELETE")
        {
            // 撤销删除 = 恢复
            if (long.TryParse(op.EntityId, out var id))
            {
                await _creatorRepository.RestoreAsync(id);
                await _creatorRepository.SaveChangesAsync();
            }
        }
        else if (op.OperationType == "UPDATE" && op.BeforeJson != null)
        {
            // 撤销更新 = 恢复原始状态
            var before = JsonSerializer.Deserialize<Creator>(op.BeforeJson, JsonOptions);
            if (before != null)
            {
                await _creatorRepository.UpdateAsync(before);
                await _creatorRepository.SaveChangesAsync();
            }
        }
    }

    private async Task UndoShipmentAsync(OperationLog op)
    {
        if (op.OperationType == "CREATE")
        {
            if (long.TryParse(op.EntityId, out var id))
            {
                await _shipmentRepository.PermanentDeleteAsync(id);
                await _shipmentRepository.SaveChangesAsync();
            }
        }
        else if (op.OperationType == "UPDATE" && op.BeforeJson != null)
        {
            var before = JsonSerializer.Deserialize<Shipment>(op.BeforeJson, JsonOptions);
            if (before != null)
            {
                await _shipmentRepository.UpdateAsync(before);
                await _shipmentRepository.SaveChangesAsync();
            }
        }
    }

    private async Task UndoVideoAsync(OperationLog op)
    {
        if (op.OperationType == "CREATE")
        {
            if (long.TryParse(op.EntityId, out var id))
            {
                await _videoRepository.PermanentDeleteAsync(id);
                await _videoRepository.SaveChangesAsync();
            }
        }
        else if (op.OperationType == "UPDATE" && op.BeforeJson != null)
        {
            var before = JsonSerializer.Deserialize<Video>(op.BeforeJson, JsonOptions);
            if (before != null)
            {
                await _videoRepository.UpdateAsync(before);
                await _videoRepository.SaveChangesAsync();
            }
        }
    }

    public async Task<IEnumerable<OperationLog>> GetRecentOperationsAsync(int count = 20)
    {
        return await _logRepository.GetRecentAsync(count);
    }
}
