using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using ClosedXML.Excel;
using TKCompass.Domain.Models;

namespace TKCompass.Excel;

/// <summary>
/// Excel 导入服务
/// 支持从观察版 Excel 导入达人主档和视频记录
/// </summary>
public class ExcelImportService
{
    // 质量等级映射表
    private static readonly Dictionary<string, string> QualityLevelMap = new()
    {
        ["S级-爆款视频"] = "S",
        ["S级"] = "S",
        ["A-小爆视频"] = "A",
        ["A级"] = "A",
        ["B-画风好质量高但出单不多"] = "B",
        ["B级"] = "B",
        ["C-配合度高但其他一般"] = "C",
        ["C级"] = "C",
        ["D-完成3视频后没必要继续跟"] = "D",
        ["D级"] = "D"
    };
    
    /// <summary>
    /// 解析 Excel 文件并生成导入预览
    /// </summary>
    public ImportPreview ParseExcel(string filePath)
    {
        var preview = new ImportPreview();
        
        using var workbook = new XLWorkbook(filePath);
        
        // 解析达人主档 Sheet
        var creatorSheet = workbook.Worksheets.FirstOrDefault(ws => ws.Name == "达人主档");
        if (creatorSheet != null)
        {
            ParseCreatorSheet(creatorSheet, preview);
        }
        else
        {
            preview.Errors.Add("未找到'达人主档'工作表");
        }
        
        // 解析视频&代码记录 Sheet
        var videoSheet = workbook.Worksheets.FirstOrDefault(ws => ws.Name == "视频&代码记录");
        if (videoSheet != null)
        {
            ParseVideoSheet(videoSheet, preview);
        }
        else
        {
            preview.Errors.Add("未找到'视频&代码记录'工作表");
        }
        
        return preview;
    }
    
    private void ParseCreatorSheet(IXLWorksheet sheet, ImportPreview preview)
    {
        int lastRow = sheet.LastRowUsed()?.RowNumber() ?? 0;
        
        for (int row = 2; row <= lastRow; row++) // 跳过表头
        {
            var importRow = new ImportPreview.CreatorImportRow { RowIndex = row };
            
            try
            {
                // A: 质量等级
                string qualityLevelRaw = sheet.Cell(row, 1).GetString().Trim();
                importRow.QualityLevel = MapQualityLevel(qualityLevelRaw);
                
                // B: 快递单号
                importRow.TrackingNumber = sheet.Cell(row, 2).GetString().Trim();
                
                // C: SKU
                importRow.Sku = sheet.Cell(row, 3).GetString().Trim();
                
                // D: 寄样日期
                var sampleDateStr = sheet.Cell(row, 4).GetString().Trim();
                if (!string.IsNullOrEmpty(sampleDateStr))
                {
                    if (DateOnly.TryParse(sampleDateStr, out var sampleDate))
                    {
                        importRow.SampleDate = sampleDate;
                    }
                    else
                    {
                        importRow.IsValid = false;
                        importRow.ErrorMessage = $"寄样日期格式错误: {sampleDateStr}";
                    }
                }
                
                // E: 达人ID (handle)
                importRow.Handle = sheet.Cell(row, 5).GetString().Trim();
                
                // F: WhatsApp
                importRow.Whatsapp = sheet.Cell(row, 6).GetString().Trim();
                
                // G: 联系位置
                importRow.ContactLocation = sheet.Cell(row, 7).GetString().Trim();
                
                // H: Email
                importRow.Email = sheet.Cell(row, 8).GetString().Trim();
                
                // I: 其他联系方式
                importRow.OtherContact = sheet.Cell(row, 9).GetString().Trim();
                
                // J: 最近联系日期
                var contactDateStr = sheet.Cell(row, 10).GetString().Trim();
                if (!string.IsNullOrEmpty(contactDateStr))
                {
                    if (DateOnly.TryParse(contactDateStr, out var contactDate))
                    {
                        importRow.LatestContactDate = contactDate;
                    }
                }
                
                // K: 视频链接（可选）
                importRow.VideoUrl = sheet.Cell(row, 11).GetString().Trim();
                if (string.IsNullOrEmpty(importRow.VideoUrl))
                {
                    importRow.VideoUrl = null;
                }
                
                // 验证必填字段
                if (string.IsNullOrEmpty(importRow.Handle))
                {
                    importRow.IsValid = false;
                    importRow.ErrorMessage = "达人ID不能为空";
                }
                else if (string.IsNullOrEmpty(importRow.QualityLevel))
                {
                    importRow.IsValid = false;
                    importRow.ErrorMessage = $"无法识别的质量等级: {qualityLevelRaw}";
                }
            }
            catch (Exception ex)
            {
                importRow.IsValid = false;
                importRow.ErrorMessage = $"解析错误: {ex.Message}";
            }
            
            preview.CreatorRows.Add(importRow);
        }
    }
    
    private void ParseVideoSheet(IXLWorksheet sheet, ImportPreview preview)
    {
        int lastRow = sheet.LastRowUsed()?.RowNumber() ?? 0;
        
        for (int row = 2; row <= lastRow; row++) // 跳过表头
        {
            var importRow = new ImportPreview.VideoImportRow { RowIndex = row };
            
            try
            {
                // A: 达人ID (handle)
                importRow.Handle = sheet.Cell(row, 1).GetString().Trim();
                
                // B: 视频链接
                importRow.VideoUrl = sheet.Cell(row, 2).GetString().Trim();
                
                // C: 视频日期
                var videoDateCell = sheet.Cell(row, 3);
                if (videoDateCell.DataType == XLDataType.DateTime)
                {
                    importRow.VideoDate = videoDateCell.GetDateTime();
                }
                else
                {
                    var videoDateStr = videoDateCell.GetString().Trim();
                    if (!string.IsNullOrEmpty(videoDateStr))
                    {
                        if (DateTime.TryParse(videoDateStr, out var videoDate))
                        {
                            importRow.VideoDate = videoDate;
                        }
                    }
                }
                
                // D: 广告代码
                importRow.AdCode = sheet.Cell(row, 4).GetString().Trim();
                if (string.IsNullOrEmpty(importRow.AdCode))
                {
                    importRow.AdCode = null;
                }
                
                // 验证必填字段
                if (string.IsNullOrEmpty(importRow.Handle))
                {
                    importRow.IsValid = false;
                    importRow.ErrorMessage = "达人ID不能为空";
                }
                else if (string.IsNullOrEmpty(importRow.VideoUrl))
                {
                    importRow.IsValid = false;
                    importRow.ErrorMessage = "视频链接不能为空";
                }
            }
            catch (Exception ex)
            {
                importRow.IsValid = false;
                importRow.ErrorMessage = $"解析错误: {ex.Message}";
            }
            
            preview.VideoRows.Add(importRow);
        }
    }
    
    private string MapQualityLevel(string raw)
    {
        if (QualityLevelMap.TryGetValue(raw, out var mapped))
        {
            return mapped;
        }
        
        // 如果已经是 S/A/B/C/D，直接返回
        if (raw.Length == 1 && "SABCD".Contains(raw.ToUpper()))
        {
            return raw.ToUpper();
        }
        
        return "";
    }
}
