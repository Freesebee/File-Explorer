using System.Globalization;
using System.Windows.Data;

namespace Lab1.Models.Converters
{
    public class SortByConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (SortBy)value == (SortBy)int.Parse((string)parameter);
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value == true ? (SortBy)int.Parse((string)parameter) : null;
        }
    }
}
