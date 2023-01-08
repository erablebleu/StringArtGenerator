using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace StringArtGenerator.App.Resources.Converters;

[ValueConversion(typeof(bool), typeof(Visibility))]
public class BoolToVisibilityConverter : IValueConverter
{
    public object? Convert(object value, Type type, object parameter, CultureInfo culture)
    {        
        return ((bool)value) ? Visibility.Visible : (Enum.TryParse<Visibility>(parameter.ToString(), out Visibility vis) ? vis : Visibility.Hidden);
    }

    public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
    {
        return (Visibility)value == Visibility.Visible;
    }
}