using MultiLogViewer.Models;
using System;
using System.Globalization;
using System.Windows.Data;

namespace MultiLogViewer.ViewModels.Converters
{
    /// <summary>
    /// LogEntry から Timestamp を抽出し、フィルタ用のパラメータとセットにするコンバーターです。
    /// </summary>
    public class DateTimeFilterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is LogEntry entry && parameter is bool isAfter)
            {
                return (entry.Timestamp, isAfter);
            }
            return null!;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
