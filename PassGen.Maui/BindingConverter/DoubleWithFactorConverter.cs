using System.Globalization;

namespace PassGen.Maui;

public sealed class DoubleWithFactorConverter : IValueConverter
{
    public double Factor { get; set; } = 1.0;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        Factor * (double) value;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        ((double) value) / Factor;
}
