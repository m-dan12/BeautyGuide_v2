using Avalonia.Data.Converters;
using Avalonia.Media;
using System;

namespace BeautyGuide_v2.Converters;

public class EmptyToRedBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (value is string str && string.IsNullOrWhiteSpace(str))
            return new SolidColorBrush(Colors.Red);
        return new SolidColorBrush(Colors.Gray); // Цвет границы по умолчанию
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}