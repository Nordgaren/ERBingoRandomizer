using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ERBingoRandomizer.Converter;

public class BoolToVisibilityConverter : IValueConverter {
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        bool val = (bool)value;

        if (val == null)
            throw new ArgumentNullException(nameof(val));

        return val ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        Visibility? visibility = (Visibility)value;

        if (visibility == null)
            throw new ArgumentNullException(nameof(visibility));

        return visibility == Visibility.Visible;
    }
}
