using System.Windows;
using HandyControl.Tools;
namespace HandyControl.Controls
{
    public static partial class WindowAttach
    {
        #region SystemBackdropType

        public static readonly DependencyProperty SystemBackdropTypeProperty =
            DependencyProperty.RegisterAttached(
                "SystemBackdropType",
                typeof(BackdropType),
                typeof(WindowAttach),
                new PropertyMetadata(BackdropType.Auto, OnSystemBackdropTypeChanged));

        public static BackdropType GetSystemBackdropType(System.Windows.Window window)
        {
            return (BackdropType) window.GetValue(SystemBackdropTypeProperty);
        }

        public static void SetSystemBackdropType(System.Windows.Window window, BackdropType value)
        {
            window.SetValue(SystemBackdropTypeProperty, value);
        }

        private static void OnSystemBackdropTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is System.Windows.Window window)
            {
                window.Apply((BackdropType) e.NewValue);
            }
        }

        #endregion
    }
}
