using System.Windows;
using System.Windows.Input;

namespace MultiLogViewer.Behaviors
{
    /// <summary>
    /// UI 要素の左ボタンクリックによるウィンドウのドラッグ移動を可能にする添付プロパティを提供します。
    /// </summary>
    public static class WindowDragBehavior
    {
        public static readonly DependencyProperty LeftButtonDragProperty =
            DependencyProperty.RegisterAttached(
                "LeftButtonDrag",
                typeof(bool),
                typeof(WindowDragBehavior),
                new PropertyMetadata(false, OnLeftButtonDragChanged));

        public static bool GetLeftButtonDrag(DependencyObject obj) => (bool)obj.GetValue(LeftButtonDragProperty);
        public static void SetLeftButtonDrag(DependencyObject obj, bool value) => obj.SetValue(LeftButtonDragProperty, value);

        private static void OnLeftButtonDragChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement element)
            {
                if ((bool)e.NewValue)
                {
                    element.MouseLeftButtonDown += Element_MouseLeftButtonDown;
                }
                else
                {
                    element.MouseLeftButtonDown -= Element_MouseLeftButtonDown;
                }
            }
        }

        private static void Element_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var window = Window.GetWindow((DependencyObject)sender);
                window?.DragMove();
            }
        }
    }
}
