using Avalonia.Data.Converters;
using System;
using System.Globalization;
using TKCompass.Domain.Enums;

namespace TKCompass.AppBase.Converters;

/// <summary>
/// 合作状态枚举转中文
/// </summary>
public class CooperationStatusConverter : IValueConverter
{
    public static readonly CooperationStatusConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is CooperationStatus status)
        {
            return status switch
            {
                CooperationStatus.Unreached => "未触达",
                CooperationStatus.Reached => "已触达",
                CooperationStatus.CooperationSent => "已发送合作",
                CooperationStatus.CooperationAgreed => "已同意合作",
                CooperationStatus.SampleSent => "已寄样",
                _ => status.ToString()
            };
        }
        return value?.ToString() ?? "";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 今日状态枚举转中文
/// </summary>
public class TodayStatusConverter : IValueConverter
{
    public static readonly TodayStatusConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is TodayStatus status)
        {
            return status switch
            {
                TodayStatus.Pending => "待跟进",
                TodayStatus.Reminded => "已提醒",
                TodayStatus.Resolved => "已解决",
                _ => status.ToString()
            };
        }
        return value == null ? "未设置" : value.ToString() ?? "";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 今日状态枚举转颜色
/// </summary>
public class TodayStatusToBrushConverter : IValueConverter
{
    public static readonly TodayStatusToBrushConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is TodayStatus status)
        {
            return status switch
            {
                TodayStatus.Pending => new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.FromRgb(0xFF, 0x98, 0x00)), // Orange
                TodayStatus.Reminded => new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.FromRgb(0x21, 0x96, 0xF3)), // Blue
                TodayStatus.Resolved => new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.FromRgb(0x4C, 0xAF, 0x50)), // Green
                _ => new Avalonia.Media.SolidColorBrush(Avalonia.Media.Colors.Gray)
            };
        }
        return new Avalonia.Media.SolidColorBrush(Avalonia.Media.Colors.Gray);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
