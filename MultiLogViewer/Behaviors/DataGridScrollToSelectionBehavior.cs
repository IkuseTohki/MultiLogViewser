using System.Windows;
using System.Windows.Controls;

namespace MultiLogViewer.Behaviors
{
    public static class DataGridScrollToSelectionBehavior
    {
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.RegisterAttached(
                "SelectedItem",
                typeof(object),
                typeof(DataGridScrollToSelectionBehavior),
                new PropertyMetadata(null, OnSelectedItemChanged));

        public static object GetSelectedItem(DependencyObject obj)
        {
            return obj.GetValue(SelectedItemProperty);
        }

        public static void SetSelectedItem(DependencyObject obj, object value)
        {
            obj.SetValue(SelectedItemProperty, value);
        }

        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataGrid dataGrid && e.NewValue != null)
            {
                dataGrid.ScrollIntoView(e.NewValue);
            }
        }
    }
}
