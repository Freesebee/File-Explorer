using System.Globalization;
using System.Windows.Data;

namespace Lab1.Models.Converters
{
    public class SortOrderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (SortOrder)value == (SortOrder)int.Parse((string)parameter);
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value == true ? (SortOrder)int.Parse((string)parameter) : null;
        }
    }
}
