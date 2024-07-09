using System.Globalization;

namespace PassGen.Maui;

public sealed class DoubleWithRatioConverter : IValueConverter
{
    public double Ratio { get; set; } = 1.0;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        (double)value * Ratio;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
