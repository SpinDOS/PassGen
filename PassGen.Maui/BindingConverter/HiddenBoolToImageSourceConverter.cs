using System.Globalization;

namespace PassGen.Maui;

public sealed class HiddenBoolToImageSourceConverter : IValueConverter
{
    public ImageSource ShowImage { get; set;}
    public ImageSource HideImage { get; set;}

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        (value != null && (bool)value) ? ShowImage : HideImage;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}