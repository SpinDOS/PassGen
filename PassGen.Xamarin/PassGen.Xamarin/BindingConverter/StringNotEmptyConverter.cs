using System;
using System.Globalization;
using Xamarin.Forms;

namespace PassGen.Xamarin.BindingConverter
{
    public sealed class StringNotEmptyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            !string.IsNullOrEmpty((string) value);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }
}