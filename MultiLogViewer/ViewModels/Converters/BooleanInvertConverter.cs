using System;
using System.Globalization;
using System.Windows.Data;

namespace MultiLogViewer.ViewModels.Converters
{
    /// <summary>
    /// bool 値を反転させるコンバーター。
    /// </summary>
    public class BooleanInvertConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
            {
                return !b;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
            {
                return !b;
            }
            return value;
        }
    }
}
