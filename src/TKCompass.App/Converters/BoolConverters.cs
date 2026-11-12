using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace TKCompass.AppBase.Converters;

/// <summary>
/// 导航选中状态转换为 Brush（背景色）
/// </summary>
public class NavigationSelectionBrushConverter : IValueConverter
{
    // 背景色转换器实例
    public static readonly NavigationSelectionBrushConverter TodayReminderBg = new() { MatchValue = "TodayReminder", MatchBrush = "#E3F2FD", NoMatchBrush = "Transparent" };
    public static readonly NavigationSelectionBrushConverter InDeliveryBg = new() { MatchValue = "InDelivery", MatchBrush = "#E3F2FD", NoMatchBrush = "Transparent" };
    public static readonly NavigationSelectionBrushConverter AllCreatorsBg = new() { MatchValue = "AllCreators", MatchBrush = "#E3F2FD", NoMatchBrush = "Transparent" };
    public static readonly NavigationSelectionBrushConverter DeliveredBg = new() { MatchValue = "Delivered", MatchBrush = "#E3F2FD", NoMatchBrush = "Transparent" };
    public static readonly NavigationSelectionBrushConverter DeletedBg = new() { MatchValue = "Deleted", MatchBrush = "#E3F2FD", NoMatchBrush = "Transparent" };
    public static readonly NavigationSelectionBrushConverter RemindedBg = new() { MatchValue = "Reminded", MatchBrush = "#E3F2FD", NoMatchBrush = "Transparent" };
    public static readonly NavigationSelectionBrushConverter DashboardBg = new() { MatchValue = "Dashboard", MatchBrush = "#E3F2FD", NoMatchBrush = "Transparent" };
    public static readonly NavigationSelectionBrushConverter CrmBg = new() { MatchValue = "Crm", MatchBrush = "#E3F2FD", NoMatchBrush = "Transparent" };
    public static readonly NavigationSelectionBrushConverter ExcelImportBg = new() { MatchValue = "ExcelImport", MatchBrush = "#E3F2FD", NoMatchBrush = "Transparent" };
    public static readonly NavigationSelectionBrushConverter ConfigBg = new() { MatchValue = "Config", MatchBrush = "#E3F2FD", NoMatchBrush = "Transparent" };
    public static readonly NavigationSelectionBrushConverter OperationHistoryBg = new() { MatchValue = "OperationHistory", MatchBrush = "#E3F2FD", NoMatchBrush = "Transparent" };
    public static readonly NavigationSelectionBrushConverter SettingsBg = new() { MatchValue = "Settings", MatchBrush = "#E3F2FD", NoMatchBrush = "Transparent" };
    
    // 边框色转换器实例
    public static readonly NavigationSelectionBrushConverter TodayReminderBorder = new() { MatchValue = "TodayReminder", MatchBrush = "#1976D2", NoMatchBrush = "Transparent" };
    public static readonly NavigationSelectionBrushConverter InDeliveryBorder = new() { MatchValue = "InDelivery", MatchBrush = "#1976D2", NoMatchBrush = "Transparent" };
    public static readonly NavigationSelectionBrushConverter AllCreatorsBorder = new() { MatchValue = "AllCreators", MatchBrush = "#1976D2", NoMatchBrush = "Transparent" };
    public static readonly NavigationSelectionBrushConverter DeliveredBorder = new() { MatchValue = "Delivered", MatchBrush = "#1976D2", NoMatchBrush = "Transparent" };
    public static readonly NavigationSelectionBrushConverter DeletedBorder = new() { MatchValue = "Deleted", MatchBrush = "#1976D2", NoMatchBrush = "Transparent" };
    public static readonly NavigationSelectionBrushConverter RemindedBorder = new() { MatchValue = "Reminded", MatchBrush = "#1976D2", NoMatchBrush = "Transparent" };
    public static readonly NavigationSelectionBrushConverter DashboardBorder = new() { MatchValue = "Dashboard", MatchBrush = "#1976D2", NoMatchBrush = "Transparent" };
    public static readonly NavigationSelectionBrushConverter CrmBorder = new() { MatchValue = "Crm", MatchBrush = "#1976D2", NoMatchBrush = "Transparent" };
    public static readonly NavigationSelectionBrushConverter ExcelImportBorder = new() { MatchValue = "ExcelImport", MatchBrush = "#1976D2", NoMatchBrush = "Transparent" };
    public static readonly NavigationSelectionBrushConverter ConfigBorder = new() { MatchValue = "Config", MatchBrush = "#1976D2", NoMatchBrush = "Transparent" };
    public static readonly NavigationSelectionBrushConverter OperationHistoryBorder = new() { MatchValue = "OperationHistory", MatchBrush = "#1976D2", NoMatchBrush = "Transparent" };
    public static readonly NavigationSelectionBrushConverter SettingsBorder = new() { MatchValue = "Settings", MatchBrush = "#1976D2", NoMatchBrush = "Transparent" };

    public string MatchValue { get; set; } = "";
    public string MatchBrush { get; set; } = "#E3F2FD";
    public string NoMatchBrush { get; set; } = "Transparent";

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string s && string.Equals(s, MatchValue, StringComparison.Ordinal))
        {
            return Avalonia.Media.Brush.Parse(MatchBrush);
        }
        return Avalonia.Media.Brush.Parse(NoMatchBrush);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// StringEqualsToBrushConverter - MainWindow.axaml 使用的静态别名类
/// </summary>
public static class StringEqualsToBrushConverter
{
    public static readonly NavigationSelectionBrushConverter TodayReminderInstance = NavigationSelectionBrushConverter.TodayReminderBg;
    public static readonly NavigationSelectionBrushConverter TodayReminderBorderInstance = NavigationSelectionBrushConverter.TodayReminderBorder;
    public static readonly NavigationSelectionBrushConverter InDeliveryInstance = NavigationSelectionBrushConverter.InDeliveryBg;
    public static readonly NavigationSelectionBrushConverter InDeliveryBorderInstance = NavigationSelectionBrushConverter.InDeliveryBorder;
    public static readonly NavigationSelectionBrushConverter AllCreatorsInstance = NavigationSelectionBrushConverter.AllCreatorsBg;
    public static readonly NavigationSelectionBrushConverter AllCreatorsBorderInstance = NavigationSelectionBrushConverter.AllCreatorsBorder;
    public static readonly NavigationSelectionBrushConverter DeliveredInstance = NavigationSelectionBrushConverter.DeliveredBg;
    public static readonly NavigationSelectionBrushConverter DeliveredBorderInstance = NavigationSelectionBrushConverter.DeliveredBorder;
    public static readonly NavigationSelectionBrushConverter DeletedInstance = NavigationSelectionBrushConverter.DeletedBg;
    public static readonly NavigationSelectionBrushConverter DeletedBorderInstance = NavigationSelectionBrushConverter.DeletedBorder;
    public static readonly NavigationSelectionBrushConverter RemindedInstance = NavigationSelectionBrushConverter.RemindedBg;
    public static readonly NavigationSelectionBrushConverter RemindedBorderInstance = NavigationSelectionBrushConverter.RemindedBorder;
    public static readonly NavigationSelectionBrushConverter DashboardInstance = NavigationSelectionBrushConverter.DashboardBg;
    public static readonly NavigationSelectionBrushConverter DashboardBorderInstance = NavigationSelectionBrushConverter.DashboardBorder;
    
    // 5个顶级导航的 Bg/Border 别名
    public static readonly NavigationSelectionBrushConverter CrmInstance = NavigationSelectionBrushConverter.CrmBg;
    public static readonly NavigationSelectionBrushConverter CrmBorderInstance = NavigationSelectionBrushConverter.CrmBorder;
    public static readonly NavigationSelectionBrushConverter ExcelImportInstance = NavigationSelectionBrushConverter.ExcelImportBg;
    public static readonly NavigationSelectionBrushConverter ExcelImportBorderInstance = NavigationSelectionBrushConverter.ExcelImportBorder;
    public static readonly NavigationSelectionBrushConverter ConfigInstance = NavigationSelectionBrushConverter.ConfigBg;
    public static readonly NavigationSelectionBrushConverter ConfigBorderInstance = NavigationSelectionBrushConverter.ConfigBorder;
    public static readonly NavigationSelectionBrushConverter OperationHistoryInstance = NavigationSelectionBrushConverter.OperationHistoryBg;
    public static readonly NavigationSelectionBrushConverter OperationHistoryBorderInstance = NavigationSelectionBrushConverter.OperationHistoryBorder;
    public static readonly NavigationSelectionBrushConverter SettingsInstance = NavigationSelectionBrushConverter.SettingsBg;
    public static readonly NavigationSelectionBrushConverter SettingsBorderInstance = NavigationSelectionBrushConverter.SettingsBorder;
}

/// <summary>
/// 将 bool 转换为 IBrush（用于 Foreground/Background）
/// </summary>
public class BoolToBrushConverter : IValueConverter
{
    public static readonly BoolToBrushConverter TrueToRed = new() { TrueBrush = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Colors.Red), FalseBrush = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Colors.Gray) };
    public static readonly BoolToBrushConverter TrueToGreen = new() { TrueBrush = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Colors.Green), FalseBrush = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Colors.Gray) };

    public Avalonia.Media.IBrush TrueBrush { get; set; } = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Colors.Green);
    public Avalonia.Media.IBrush FalseBrush { get; set; } = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Colors.Red);

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b)
        {
            return b ? TrueBrush : FalseBrush;
        }
        return FalseBrush;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 将 bool 转换为颜色字符串
/// </summary>
public class BoolToColorConverter : IValueConverter
{
    public static readonly BoolToColorConverter Red = new() { TrueColor = "#FF0000", FalseColor = "#808080" };
    public static readonly BoolToColorConverter Green = new() { TrueColor = "#00FF00", FalseColor = "#808080" };

    public string TrueColor { get; set; } = "#00FF00";
    public string FalseColor { get; set; } = "#FF0000";

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b)
        {
            return b ? TrueColor : FalseColor;
        }
        return FalseColor;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 将 bool 转换为 Background Brush（True=红色, False=绿色）
/// </summary>
public class BoolToBackgroundConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b)
        {
            return b ? Avalonia.Media.Brush.Parse("#F44336") : Avalonia.Media.Brush.Parse("#4CAF50");
        }
        return Avalonia.Media.Brush.Parse("#4CAF50");
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 将 bool 转换为字符串（Yes/No）
/// </summary>
public class BoolToStringConverter : IValueConverter
{
    public static readonly BoolToStringConverter YesNo = new() { TrueString = "Yes", FalseString = "No" };
    public static readonly BoolToStringConverter EnabledDisabled = new() { TrueString = "Enabled", FalseString = "Disabled" };

    public string TrueString { get; set; } = "Yes";
    public string FalseString { get; set; } = "No";

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b)
        {
            return b ? TrueString : FalseString;
        }
        return FalseString;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
