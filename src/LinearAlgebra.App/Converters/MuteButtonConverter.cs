using System.Globalization;
using System.Windows.Data;

namespace LinearAlgebra.App.Converters;

public class MuteButtonConverter : IValueConverter
{
    public static readonly MuteButtonConverter Instance = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is true ? "Unmute" : "Mute";

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
