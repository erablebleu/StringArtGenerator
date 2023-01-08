using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace StringArtGenerator.App.Resources.Converters;

public class ItemToListIndexConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length < 2
            || values[1] is not IList list
            || !list.Contains(values[0])) return null;
        _ = int.TryParse(parameter?.ToString(), out int off);
        return list.IndexOf(values[0]) + off;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}