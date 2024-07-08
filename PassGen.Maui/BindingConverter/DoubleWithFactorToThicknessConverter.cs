using System.Globalization;

namespace PassGen.Maui;

public sealed class DoubleWithFactorToThicknessConverter : IValueConverter
{
    public Thickness ThicknessFactor { get; set; } = 1.0;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var d = (double) value;
        return new Thickness(ThicknessFactor.Left * d, ThicknessFactor.Top * d, ThicknessFactor.Right * d, ThicknessFactor.Bottom * d);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
