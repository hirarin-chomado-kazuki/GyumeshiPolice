using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace GyumeshiPolice.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public bool IsInverse { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var b = (bool)value;
            if (this.IsInverse)
            {
                b = !b;
            }
            return b ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var v = (Visibility)value;
            if (this.IsInverse)
            {
                return v == Visibility.Collapsed;
            }
            else
            {
                return v == Visibility.Visible;
            }
        }
    }
}
