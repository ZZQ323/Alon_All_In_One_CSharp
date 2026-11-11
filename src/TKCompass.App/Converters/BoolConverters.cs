using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace TKCompass.AppBase.Converters;

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
