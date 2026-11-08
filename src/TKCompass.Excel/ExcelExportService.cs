using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using ClosedXML.Excel;
using TKCompass.Domain.Models;

namespace TKCompass.Excel;

/// <summary>
/// Excel 导出服务
/// </summary>
public class ExcelExportService
{
    /// <summary>
    /// 导出达人列表到 Excel
    /// </summary>
    public void ExportCreatorList(IEnumerable<Creator> creators, string filePath)
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("达人列表");
        
        // 表头样式
        var headerStyle = sheet.Row(1).Style;
        headerStyle.Font.Bold = true;
        headerStyle.Fill.BackgroundColor = XLColor.LightBlue;
        headerStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        
        // 写入表头
        string[] headers = { "达人ID", "昵称", "质量等级", "WhatsApp", "Email", "其他联系方式",
                            "首次寄样日期", "最近联系日期", "备注" };
        for (int i = 0; i < headers.Length; i++)
        {
            sheet.Cell(1, i + 1).Value = headers[i];
        }
        
        // 写入数据
        int row = 2;
        foreach (var creator in creators)
        {
            sheet.Cell(row, 1).Value = creator.Handle;
            sheet.Cell(row, 2).Value = creator.Nickname;
            sheet.Cell(row, 3).Value = creator.QualityLevel;
            
            // 联系方式（从 Contacts 集合中获取）
            var whatsapp = creator.Contacts?.FirstOrDefault(c => c.ContactType == Domain.Enums.ContactType.WhatsApp);
            sheet.Cell(row, 4).Value = whatsapp?.ContactValue ?? "";
            
            var email = creator.Contacts?.FirstOrDefault(c => c.ContactType == Domain.Enums.ContactType.Email);
            sheet.Cell(row, 5).Value = email?.ContactValue ?? "";
            
            var other = creator.Contacts?.FirstOrDefault(c => c.ContactType == Domain.Enums.ContactType.Other);
            sheet.Cell(row, 6).Value = other?.ContactValue ?? "";
            
            sheet.Cell(row, 7).Value = creator.FirstSampleDate?.ToString("yyyy-MM-dd") ?? "";
            sheet.Cell(row, 8).Value = creator.LatestContactDate?.ToString("yyyy-MM-dd") ?? "";
            sheet.Cell(row, 9).Value = creator.CreatorNotes ?? "";
            
            row++;
        }
        
        // 自动调整列宽
        sheet.Columns().AdjustToContents();
        
        // 保存文件
        workbook.SaveAs(filePath);
    }
    
    /// <summary>
    /// 导出达人详情（包含寄样和视频）到 Excel
    /// </summary>
    public void ExportCreatorDetail(Creator creator, IEnumerable<Shipment> shipments, 
                                    IEnumerable<Video> videos, string filePath)
    {
        using var workbook = new XLWorkbook();
        
        // 达人基本信息 Sheet
        var infoSheet = workbook.Worksheets.Add("达人信息");
        WriteCreatorInfo(creator, infoSheet);
        
        // 寄样记录 Sheet
        var shipmentSheet = workbook.Worksheets.Add("寄样记录");
        WriteShipments(shipments, shipmentSheet);
        
        // 视频记录 Sheet
        var videoSheet = workbook.Worksheets.Add("视频记录");
        WriteVideos(videos, videoSheet);
        
        // 保存文件
        workbook.SaveAs(filePath);
    }
    
    private void WriteCreatorInfo(Creator creator, IXLWorksheet sheet)
    {
        // 表头样式
        var headerStyle = sheet.Row(1).Style;
        headerStyle.Font.Bold = true;
        headerStyle.Fill.BackgroundColor = XLColor.LightBlue;
        
        // 写入基本信息
        sheet.Cell(1, 1).Value = "达人ID";
        sheet.Cell(1, 2).Value = creator.Handle;
        
        sheet.Cell(2, 1).Value = "昵称";
        sheet.Cell(2, 2).Value = creator.Nickname;
        
        sheet.Cell(3, 1).Value = "质量等级";
        sheet.Cell(3, 2).Value = creator.QualityLevel;
        
        sheet.Cell(4, 1).Value = "合作状态";
        sheet.Cell(4, 2).Value = creator.CooperationStatus.ToString();
        
        sheet.Cell(5, 1).Value = "是否已交付";
        sheet.Cell(5, 2).Value = creator.Delivered ? "是" : "否";
        
        sheet.Cell(6, 1).Value = "首次寄样日期";
        sheet.Cell(6, 2).Value = creator.FirstSampleDate?.ToString("yyyy-MM-dd") ?? "";
        
        sheet.Cell(7, 1).Value = "最近联系日期";
        sheet.Cell(7, 2).Value = creator.LatestContactDate?.ToString("yyyy-MM-dd") ?? "";
        
        sheet.Cell(8, 1).Value = "TikTok主页";
        sheet.Cell(8, 2).Value = creator.TiktokHomepageUrl ?? "";
        
        // 联系方式
        int row = 10;
        sheet.Cell(row, 1).Value = "联系方式";
        sheet.Cell(row, 1).Style.Font.Bold = true;
        row++;
        
        if (creator.Contacts != null)
        {
            foreach (var contact in creator.Contacts)
            {
                sheet.Cell(row, 1).Value = contact.ContactType.ToString();
                sheet.Cell(row, 2).Value = contact.ContactValue;
                row++;
            }
        }
        
        // 自动调整列宽
        sheet.Columns().AdjustToContents();
    }
    
    private void WriteShipments(IEnumerable<Shipment> shipments, IXLWorksheet sheet)
    {
        // 表头
        string[] headers = { "寄样ID", "快递单号", "SKU", "产品名称", "寄样日期", 
                            "要求视频数", "是否完成", "备注" };
        
        var headerStyle = sheet.Row(1).Style;
        headerStyle.Font.Bold = true;
        headerStyle.Fill.BackgroundColor = XLColor.LightBlue;
        
        for (int i = 0; i < headers.Length; i++)
        {
            sheet.Cell(1, i + 1).Value = headers[i];
        }
        
        // 写入数据
        int row = 2;
        foreach (var shipment in shipments)
        {
            sheet.Cell(row, 1).Value = shipment.Id;
            sheet.Cell(row, 2).Value = shipment.TrackingNumber ?? "";
            sheet.Cell(row, 3).Value = shipment.Sku ?? "";
            sheet.Cell(row, 4).Value = shipment.ProductName ?? "";
            sheet.Cell(row, 5).Value = shipment.SampleDate?.ToString("yyyy-MM-dd") ?? "";
            sheet.Cell(row, 6).Value = shipment.RequiredVideoCount;
            sheet.Cell(row, 7).Value = shipment.IsCompletedManual ? "是" : "否";
            sheet.Cell(row, 8).Value = shipment.Notes ?? "";
            
            row++;
        }
        
        // 自动调整列宽
        sheet.Columns().AdjustToContents();
    }
    
    private void WriteVideos(IEnumerable<Video> videos, IXLWorksheet sheet)
    {
        // 表头
        string[] headers = { "视频ID", "寄样ID", "视频链接", "视频日期", "广告代码", 
                            "观看数", "订单数", "GMV", "是否有效" };
        
        var headerStyle = sheet.Row(1).Style;
        headerStyle.Font.Bold = true;
        headerStyle.Fill.BackgroundColor = XLColor.LightBlue;
        
        for (int i = 0; i < headers.Length; i++)
        {
            sheet.Cell(1, i + 1).Value = headers[i];
        }
        
        // 写入数据
        int row = 2;
        foreach (var video in videos)
        {
            sheet.Cell(row, 1).Value = video.Id;
            sheet.Cell(row, 2).Value = video.ShipmentId ?? 0;
            sheet.Cell(row, 3).Value = video.VideoUrl ?? "";
            sheet.Cell(row, 4).Value = video.VideoDate?.ToString("yyyy-MM-dd HH:mm") ?? "";
            sheet.Cell(row, 5).Value = video.AdCode ?? "";
            sheet.Cell(row, 6).Value = video.Views;
            sheet.Cell(row, 7).Value = video.Orders;
            sheet.Cell(row, 8).Value = video.Gmv;
            sheet.Cell(row, 9).Value = video.IsEffective ? "是" : "否";
            
            row++;
        }
        
        // 自动调整列宽
        sheet.Columns().AdjustToContents();
    }
}
