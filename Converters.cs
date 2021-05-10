using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Brush = System.Windows.Media.Brush;

namespace WpfAppReports
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BooleanVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (!(value is bool))
                throw new InvalidOperationException("The source must be a bool");
            if (targetType != typeof(Visibility))
                throw new InvalidOperationException("The target must be a Visibiity");

            return (bool)value ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    [ValueConversion(typeof(string), typeof(Brush))]
    public class StringBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (!(value is string))
                throw new InvalidOperationException("The source must be a string");

            string brushString = value.ToString();

            var brush = new BrushConverter().ConvertFromString(brushString) as SolidColorBrush;
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
