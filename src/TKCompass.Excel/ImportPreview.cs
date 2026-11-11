using System;
using System.Collections.Generic;
using TKCompass.Domain.Models;

namespace TKCompass.Excel;

/// <summary>
/// 导入预览数据传输对象
/// </summary>
public class ImportPreview
{
    public List<CreatorImportRow> CreatorRows { get; set; } = new();
    public List<VideoImportRow> VideoRows { get; set; } = new();
    public List<string> Errors { get; set; } = new();
    
    // 兼容旧代码的属性
    public List<CreatorImportRow> Creators => CreatorRows;
    public List<VideoImportRow> Videos => VideoRows;
    public bool HasErrors => Errors.Count > 0;
    
    public int ValidCreatorCount => CreatorRows.FindAll(r => r.IsValid).Count;
    public int ValidVideoCount => VideoRows.FindAll(r => r.IsValid).Count;
    public int ErrorCount => Errors.Count;
    
    /// <summary>
    /// 达人主档导入行
    /// </summary>
    public class CreatorImportRow
    {
        public int RowIndex { get; set; }
        public string QualityLevel { get; set; } = "";
        public string TrackingNumber { get; set; } = "";
        public string Sku { get; set; } = "";
        public DateOnly SampleDate { get; set; }
        public string Handle { get; set; } = "";
        public string Whatsapp { get; set; } = "";
        public string ContactLocation { get; set; } = "";
        public string Email { get; set; } = "";
        public string OtherContact { get; set; } = "";
        public DateOnly? LatestContactDate { get; set; }
        public string? VideoUrl { get; set; }
        public bool IsValid { get; set; } = true;
        public string? ErrorMessage { get; set; }
    }
    
    /// <summary>
    /// 视频记录导入行
    /// </summary>
    public class VideoImportRow
    {
        public int RowIndex { get; set; }
        public string Handle { get; set; } = "";
        public string VideoUrl { get; set; } = "";
        public DateTime? VideoDate { get; set; }
        public string? AdCode { get; set; }
        public bool IsValid { get; set; } = true;
        public string? ErrorMessage { get; set; }
    }
}
