using System;
using Avalonia.Controls;
using Avalonia.Data.Converters;

namespace BeautyGuide_v2.Converters;

public class TagToValueConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (value is ComboBoxItem item)
        {
            return item.Tag;
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}