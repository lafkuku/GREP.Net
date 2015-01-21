using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Grep.Net.WPF.Client.Converters
{
    public class SubtractionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int && parameter is int)
                return (int)value - (int)parameter;

            throw new InvalidCastException("Invalid paramters to Converter, cannot cast to Int");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}