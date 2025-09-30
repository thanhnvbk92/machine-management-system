using System;
using System.Globalization;
using System.Windows.Data;

namespace MachineClient.WPF.Converters;

public class BooleanToInverseConverter : IValueConverter
{
    public static BooleanToInverseConverter Instance { get; } = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return !boolValue;
        }
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return !boolValue;
        }
        return false;
    }
}