using System.Globalization;
using System.Windows.Data;

namespace Lab1
{
    public class LangBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (string)value == (string)parameter;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value == true ? (string)parameter : null;
        }
    }
}
