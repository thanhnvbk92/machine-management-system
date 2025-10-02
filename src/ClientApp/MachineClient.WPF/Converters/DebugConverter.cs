using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace MachineClient.WPF.Converters
{
    public class DebugConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var debugMessage = $"{DateTime.Now:HH:mm:ss} - DebugConverter: {parameter} = {value} (Type: {value?.GetType().Name})\n";
            File.AppendAllText("converter_debug.log", debugMessage);
            
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}