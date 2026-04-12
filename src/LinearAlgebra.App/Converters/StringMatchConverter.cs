using System.Globalization;
using System.Windows.Data;

namespace LinearAlgebra.App.Converters;

public class StringMatchConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is string s && parameter is string p && s == p;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is true && parameter is string p)
            return p;
        return Binding.DoNothing;
    }
}
