using System.Globalization;

namespace PassGen.Maui;

public sealed class DoubleToThicknessWithRatioConverter : IValueConverter
{
    public Thickness Ratio { get; set; } = 1.0;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var d = (double)value;
        return new Thickness(Ratio.Left * d, Ratio.Top * d, Ratio.Right * d, Ratio.Bottom * d);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
