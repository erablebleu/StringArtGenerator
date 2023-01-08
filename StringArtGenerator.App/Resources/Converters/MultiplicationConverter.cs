using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace StringArtGenerator.App.Resources.Converters;

[ValueConversion(typeof(double), typeof(double))]
public class MultiplicationConverter : IValueConverter
{
    private static double GetValue(object v)
    {
        if (v is double d) return d;
        if (double.TryParse(v.ToString(), NumberStyles.Number, CultureInfo.InvariantCulture, out double c)) return c;
        return 0d;
    }
    public object? Convert(object value, Type type, object parameter, CultureInfo culture) 
        => (GetValue(value) * GetValue(parameter)).ToString(CultureInfo.InvariantCulture);

    public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
        => (GetValue(value) / GetValue(parameter)).ToString(CultureInfo.InvariantCulture);
}