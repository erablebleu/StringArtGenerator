using System;
using System.Globalization;
using System.Windows.Data;

namespace StringArtGenerator.App.Resources.Converters;

[ValueConversion(typeof(object), typeof(bool))]
public class EqualityConverter : IValueConverter
{
    public object? Convert(object value, Type type, object parameter, CultureInfo culture)
    {
        return value.Equals(parameter);
    }

    public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
    {
        return ((bool)value) ? parameter : Binding.DoNothing;
    }
}