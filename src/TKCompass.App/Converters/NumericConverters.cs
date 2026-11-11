using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace TKCompass.AppBase.Converters;

/// <summary>
/// 数值比较转换器集合
/// </summary>
public static class NumericConverters
{
    /// <summary>
    /// 判断数值是否等于指定参数
    /// </summary>
    public static readonly IValueConverter Equal = new NumericEqualConverter();
}

/// <summary>
/// 将数值与 ConverterParameter 比较，相等时返回 true
/// </summary>
public class NumericEqualConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null && parameter == null)
            return true;

        if (value == null || parameter == null)
            return false;

        try
        {
            var targetValue = System.Convert.ToDouble(value, culture);
            var compareValue = System.Convert.ToDouble(parameter, culture);
            return Math.Abs(targetValue - compareValue) < double.Epsilon;
        }
        catch
        {
            return false;
        }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
