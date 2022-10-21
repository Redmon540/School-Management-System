using System.Windows;

namespace School_Manager
{
    public static class CenterOnSizeChangeBehaviour
    {
        /// <summary>
        /// Centers the window in the screen, when its changing its size.
        /// </summary>
        public static readonly DependencyProperty CenterOnSizeChangeProperty =
            DependencyProperty.RegisterAttached
                (
                "CenterOnSizeChange",
                typeof(bool),
                typeof(CenterOnSizeChangeBehaviour),
                new UIPropertyMetadata(false, OnCenterOnSizeChangePropertyChanged)
                );

        public static bool GetCenterOnSizeChange(DependencyObject obj)
        {
            return (bool)obj.GetValue(CenterOnSizeChangeProperty);
        }
        public static void SetCenterOnSizeChange(DependencyObject obj, bool value)
        {
            obj.SetValue(CenterOnSizeChangeProperty, value);
        }

        private static void OnCenterOnSizeChangePropertyChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            System.Windows.Window window = dpo as System.Windows.Window;
            if (window != null)
            {
                if ((bool)args.NewValue)
                {
                    window.SizeChanged += OnWindowSizeChanged;
                }
                else
                {
                    window.SizeChanged -= OnWindowSizeChanged;
                }
            }
        }

        private static void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            System.Windows.Window window = (System.Windows.Window)sender;

            window.WindowStartupLocation = WindowStartupLocation.Manual;
            window.Left = (SystemParameters.WorkArea.Width - window.ActualWidth) / 2 + SystemParameters.WorkArea.Left;
            window.Top = (SystemParameters.WorkArea.Height - window.ActualHeight) / 2 + SystemParameters.WorkArea.Top;
        }
    }
}
