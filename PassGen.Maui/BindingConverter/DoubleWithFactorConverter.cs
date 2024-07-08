using System.Globalization;

namespace PassGen.Maui;

public sealed class DoubleWithFactorConverter : IValueConverter
{
    public double Factor { get; set; } = 1.0;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        (double)value * Factor;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
