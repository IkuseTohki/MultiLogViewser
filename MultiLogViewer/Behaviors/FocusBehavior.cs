using System;
using System.Windows;

namespace MultiLogViewer.Behaviors
{
    public static class FocusBehavior
    {
        // FocusOnLoad
        public static readonly DependencyProperty FocusOnLoadProperty =
            DependencyProperty.RegisterAttached(
                "FocusOnLoad",
                typeof(bool),
                typeof(FocusBehavior),
                new PropertyMetadata(false, OnFocusOnLoadChanged));

        public static bool GetFocusOnLoad(DependencyObject obj)
        {
            return (bool)obj.GetValue(FocusOnLoadProperty);
        }

        public static void SetFocusOnLoad(DependencyObject obj, bool value)
        {
            obj.SetValue(FocusOnLoadProperty, value);
        }

        private static void OnFocusOnLoadChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement element && (bool)e.NewValue)
            {
                element.Loaded += (s, args) => element.Focus();
            }
        }

        // FocusOnWindowActivation
        public static readonly DependencyProperty FocusOnWindowActivationProperty =
            DependencyProperty.RegisterAttached(
                "FocusOnWindowActivation",
                typeof(bool),
                typeof(FocusBehavior),
                new PropertyMetadata(false, OnFocusOnWindowActivationChanged));

        public static bool GetFocusOnWindowActivation(DependencyObject obj)
        {
            return (bool)obj.GetValue(FocusOnWindowActivationProperty);
        }

        public static void SetFocusOnWindowActivation(DependencyObject obj, bool value)
        {
            obj.SetValue(FocusOnWindowActivationProperty, value);
        }

        private static void OnFocusOnWindowActivationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement element && (bool)e.NewValue)
            {
                element.Loaded += Element_Loaded_ForActivation;
            }
        }

        private static void Element_Loaded_ForActivation(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                var window = Window.GetWindow(element);
                if (window != null)
                {
                    void OnActivated(object? s, EventArgs args)
                    {
                        if (element.IsVisible)
                        {
                            element.Focus();
                        }
                    }

                    window.Activated += OnActivated;

                    element.Unloaded += (s, args) =>
                    {
                        window.Activated -= OnActivated;
                    };
                }

                // Remove the Loaded handler to prevent multiple registrations if Loaded fires multiple times (though unlikely for same instance without Unloaded)
                element.Loaded -= Element_Loaded_ForActivation;
            }
        }
    }
}
