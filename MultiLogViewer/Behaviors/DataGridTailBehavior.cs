using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MultiLogViewer.Behaviors
{
    public static class DataGridTailBehavior
    {
        public static readonly DependencyProperty IsAtBottomProperty =
            DependencyProperty.RegisterAttached(
                "IsAtBottom",
                typeof(bool),
                typeof(DataGridTailBehavior),
                new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static bool GetIsAtBottom(DependencyObject obj) => (bool)obj.GetValue(IsAtBottomProperty);
        public static void SetIsAtBottom(DependencyObject obj, bool value) => obj.SetValue(IsAtBottomProperty, value);

        public static readonly DependencyProperty MonitorScrollProperty =
            DependencyProperty.RegisterAttached(
                "MonitorScroll",
                typeof(bool),
                typeof(DataGridTailBehavior),
                new PropertyMetadata(false, OnMonitorScrollChanged));

        public static bool GetMonitorScroll(DependencyObject obj) => (bool)obj.GetValue(MonitorScrollProperty);
        public static void SetMonitorScroll(DependencyObject obj, bool value) => obj.SetValue(MonitorScrollProperty, value);

        private static void OnMonitorScrollChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataGrid dataGrid)
            {
                if ((bool)e.NewValue)
                {
                    dataGrid.Loaded += (s, ev) => HookScrollEvent(dataGrid);
                }
            }
        }

        private static void HookScrollEvent(DataGrid dataGrid)
        {
            var scrollViewer = GetVisualChild<ScrollViewer>(dataGrid);
            if (scrollViewer != null)
            {
                scrollViewer.ScrollChanged += (s, e) =>
                {
                    // 最下部に近い（あるいは最下部）場合に true とする
                    bool atBottom = scrollViewer.VerticalOffset >= scrollViewer.ScrollableHeight - 1.0;
                    SetIsAtBottom(dataGrid, atBottom);
                };
            }
        }

        private static T? GetVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child != null && child is T t) return t;
                var result = GetVisualChild<T>(child!);
                if (result != null) return result;
            }
            return null;
        }
    }
}
