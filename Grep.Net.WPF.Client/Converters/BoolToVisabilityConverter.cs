using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Grep.Net.WPF.Client.Converters
{
    public class BoolToVisabilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool ret = false;
            
            ret = (bool)value;

            if (parameter != null && ((string)parameter).ToLower() == "not")
            {
                ret = !ret;
            }

            if (ret)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}