using System.Globalization;
using System.Windows.Data;

namespace LinearAlgebra.App.Converters;

public class QuizButtonTextConverter : IValueConverter
{
    public static readonly QuizButtonTextConverter Instance = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is true ? "Exit Quiz" : "Test Yourself";

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
