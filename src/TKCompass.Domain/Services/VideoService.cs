using TKCompass.Domain.Models;
using TKCompass.Domain.Repositories;

namespace TKCompass.Domain.Services;

public class VideoService : IVideoService
{
    private readonly IVideoRepository _videoRepository;
    private readonly IOperationLogService _operationLogService;

    public VideoService(
        IVideoRepository videoRepository,
        IOperationLogService operationLogService)
    {
        _videoRepository = videoRepository;
        _operationLogService = operationLogService;
    }

    public async Task<Video> CreateVideoAsync(long creatorId, string videoUrl)
    {
        if (creatorId <= 0)
            throw new ArgumentException("Creator ID must be positive", nameof(creatorId));
        if (string.IsNullOrWhiteSpace(videoUrl))
            throw new ArgumentException("Video URL cannot be empty", nameof(videoUrl));

        var video = new Video
        {
            CreatorId = creatorId,
            VideoUrl = videoUrl,
            CreatedAt = DateTime.Now
        };

        var saved = await _videoRepository.AddAsync(video);
        await _videoRepository.SaveChangesAsync();

        await _operationLogService.LogOperationAsync(
            "CREATE", "Video", saved.Id.ToString(),
            null, System.Text.Json.JsonSerializer.Serialize(saved),
            null, $"创建视频 #{saved.Id}（达人ID: {creatorId}）");

        return saved;
    }

    public async Task<Video?> GetByIdAsync(long id)
    {
        return await _videoRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Video>> GetByCreatorIdAsync(long creatorId)
    {
        return await _videoRepository.GetByCreatorIdAsync(creatorId);
    }

    public async Task<IEnumerable<Video>> GetByShipmentIdAsync(long shipmentId)
    {
        return await _videoRepository.GetByShipmentIdAsync(shipmentId);
    }

    public async Task<IEnumerable<Video>> GetAllAsync()
    {
        return await _videoRepository.GetAllAsync();
    }

    public async Task<Video> UpdateAsync(Video video)
    {
        if (video.Id <= 0)
            throw new ArgumentException("Video ID must be positive", nameof(video));

        var existing = await _videoRepository.GetByIdAsync(video.Id);
        if (existing == null)
            throw new InvalidOperationException($"Video not found: {video.Id}");

        var beforeJson = System.Text.Json.JsonSerializer.Serialize(existing);
        
        await _videoRepository.UpdateAsync(video);
        await _videoRepository.SaveChangesAsync();

        await _operationLogService.LogOperationAsync(
            "UPDATE", "Video", video.Id.ToString(),
            beforeJson, System.Text.Json.JsonSerializer.Serialize(video),
            null, $"编辑视频 #{video.Id}");

        return video;
    }

    public async Task DeleteAsync(long id)
    {
        var video = await _videoRepository.GetByIdAsync(id);
        if (video == null)
            throw new InvalidOperationException($"Video not found: {id}");

        var beforeJson = System.Text.Json.JsonSerializer.Serialize(video);
        
        video.Deleted = true;
        await _videoRepository.UpdateAsync(video);
        await _videoRepository.SaveChangesAsync();

        await _operationLogService.LogOperationAsync(
            "DELETE", "Video", id.ToString(),
            beforeJson, null,
            null, $"删除视频 #{id}");
    }

    public async Task PermanentDeleteAsync(long id)
    {
        await _videoRepository.PermanentDeleteAsync(id);
        await _videoRepository.SaveChangesAsync();
    }
}
