using System.Globalization;

namespace PassGen.Maui;

public sealed class StringNotEmptyConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        !string.IsNullOrEmpty((string) value);

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
