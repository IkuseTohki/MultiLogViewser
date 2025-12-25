using MultiLogViewer.Models;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MultiLogViewer.ViewModels.Converters
{
    public class BookmarkColorToBrushConverter : IValueConverter
    {
        private static readonly SolidColorBrush BlackBrush = new SolidColorBrush(Colors.Black);
        private static readonly SolidColorBrush RedBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E57373"));
        private static readonly SolidColorBrush BlueBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#64B5F6"));
        private static readonly SolidColorBrush GreenBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#81C784"));
        private static readonly SolidColorBrush YellowBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF176"));

        static BookmarkColorToBrushConverter()
        {
            BlackBrush.Freeze();
            RedBrush.Freeze();
            BlueBrush.Freeze();
            GreenBrush.Freeze();
            YellowBrush.Freeze();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return BlackBrush;
            }

            if (value is BookmarkColor color)
            {
                return color switch
                {
                    BookmarkColor.Red => RedBrush,
                    BookmarkColor.Blue => BlueBrush,
                    BookmarkColor.Green => GreenBrush,
                    BookmarkColor.Yellow => YellowBrush,
                    _ => BlueBrush,
                };
            }
            return BlueBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
