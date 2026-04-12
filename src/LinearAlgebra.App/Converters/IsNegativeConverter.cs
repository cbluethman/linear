using System.Globalization;
using System.Windows.Data;

namespace LinearAlgebra.App.Converters;

public class IsNegativeConverter : IValueConverter
{
    public static readonly IsNegativeConverter Instance = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is double d && d < 0;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
