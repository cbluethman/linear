using System.Globalization;
using System.Windows.Data;

namespace LinearAlgebra.App.Converters;

public class IndexToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int index && parameter is string paramStr && int.TryParse(paramStr, out var target))
            return index == target;
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is true && parameter is string paramStr && int.TryParse(paramStr, out var target))
            return target;
        return Binding.DoNothing;
    }
}
