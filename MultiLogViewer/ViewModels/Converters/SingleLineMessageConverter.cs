using System;
using System.Globalization;
using System.Windows.Data;

namespace MultiLogViewer.ViewModels.Converters
{
    public class SingleLineMessageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string message && !string.IsNullOrEmpty(message))
            {
                // \r\n, \n, \r のいずれでも正しく最初の行を抽出できるようにする
                var index = message.IndexOfAny(new[] { '\r', '\n' });
                if (index >= 0)
                {
                    return message.Substring(0, index);
                }
                return message;
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
