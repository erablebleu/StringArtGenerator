using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace StringArtGenerator.App.Resources.Converters;

[ValueConversion(typeof(IEnumerable<string>), typeof(string))]
public class StringJoinConverter : IValueConverter
{
    public object? Convert(object value, Type type, object parameter, CultureInfo culture)
    {
        string? separator = parameter as string;

        if (value is not IEnumerable<int> enumerable)
            return null;

        return string.Join(separator, enumerable);
    }

    public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        => ((string)value).Split(parameter as string);
}