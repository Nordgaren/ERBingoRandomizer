using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Project.Converter;

public class InverseBoolToVisibilityConverter : IValueConverter {
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {

        if (value is bool val) {
            return val ? Visibility.Hidden : Visibility.Visible;
        }

        throw new ArgumentNullException(nameof(value));
    }

    public object ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture) {

        if (value is Visibility visibility) {
            return visibility == Visibility.Hidden;
        }

        throw new ArgumentNullException(nameof(value));
    }
}
