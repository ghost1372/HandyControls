using HandyControl.Data;
using HandyControl.Themes;
using HandyControl.Tools;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

namespace HandyControl.Controls
{
    public partial class Window
    {
        public static readonly DependencyProperty ExtendViewIntoNonClientAreaProperty = DependencyProperty.Register(
                "ExtendViewIntoNonClientArea", typeof(bool), typeof(Window),
                new PropertyMetadata(ValueBoxes.FalseBox));

        public bool ExtendViewIntoNonClientArea
        {
            get => (bool) GetValue(ExtendViewIntoNonClientAreaProperty);
            set => SetValue(ExtendViewIntoNonClientAreaProperty, ValueBoxes.BooleanBox(value));
        }

        #region Mica

        private IntPtr windowHandle;

        public static readonly DependencyProperty ApplyBackdropMaterialProperty = DependencyProperty.Register(
            "ApplyBackdropMaterial", typeof(bool), typeof(Window),
            new PropertyMetadata(ValueBoxes.FalseBox, OnApplyBackdropMaterialChanged));

        public bool ApplyBackdropMaterial
        {
            get => (bool) GetValue(ApplyBackdropMaterialProperty);
            set => SetValue(ApplyBackdropMaterialProperty, ValueBoxes.BooleanBox(value));
        }

        private static void OnApplyBackdropMaterialChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctl = (Window) d;
            ctl.InitMica();
        }

        private void InitMica()
        {
            if (ApplyBackdropMaterial && OSVersionHelper.IsWindows11_OrGreater)
            {
                this.Background = Brushes.Transparent;
                WindowStyle = WindowStyle.None;
                NonClientAreaBackground = Brushes.Transparent;
                ThemeManager.Current.ActualApplicationThemeChanged += ActualApplicationThemeChanged;
            }
        }
        private void ActualApplicationThemeChanged(ThemeManager sender, object args)
        {
            if (windowHandle != null)
            {
                if (sender.ApplicationTheme == ApplicationTheme.Light)
                {
                    WindowHelper.EnableMicaEffect(windowHandle, false);
                }
                else
                {
                    WindowHelper.EnableMicaEffect(windowHandle, true);
                }
            }
        }
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            UpdateWindowEffect(this);
        }

        protected override void OnDeactivated(EventArgs e)
        {
            base.OnDeactivated(e);
            UpdateWindowEffect(this);
        }

        public void UpdateWindowEffect(Window window)
        {
            if (ApplyBackdropMaterial && OSVersionHelper.IsWindows11_OrGreater)
            {
                UpdateWindowEffect(new WindowInteropHelper(window).EnsureHandle());
            }
        }

        public void UpdateWindowEffect(IntPtr windowHandle)
        {
            var isDark = ThemeManager.Current.ApplicationTheme == ApplicationTheme.Dark;
            WindowHelper.EnableMicaEffect(windowHandle, isDark);
        }

        #endregion

        #region Show/Hide NonClientArea Buttons
        public static readonly DependencyProperty ShowMinButtonProperty = DependencyProperty.Register(
            "ShowMinButton", typeof(bool), typeof(Window),
            new PropertyMetadata(ValueBoxes.TrueBox));

        public bool ShowMinButton
        {
            get => (bool) GetValue(ShowMinButtonProperty);
            set => SetValue(ShowMinButtonProperty, ValueBoxes.BooleanBox(value));
        }

        public static readonly DependencyProperty ShowMaxButtonProperty = DependencyProperty.Register(
            "ShowMaxButton", typeof(bool), typeof(Window),
            new PropertyMetadata(ValueBoxes.TrueBox));

        public bool ShowMaxButton
        {
            get => (bool) GetValue(ShowMaxButtonProperty);
            set => SetValue(ShowMaxButtonProperty, ValueBoxes.BooleanBox(value));
        }

        public static readonly DependencyProperty ShowCloseButtonProperty = DependencyProperty.Register(
            "ShowCloseButton", typeof(bool), typeof(Window),
            new PropertyMetadata(ValueBoxes.TrueBox));

        public bool ShowCloseButton
        {
            get => (bool) GetValue(ShowCloseButtonProperty);
            set => SetValue(ShowCloseButtonProperty, ValueBoxes.BooleanBox(value));
        }
        #endregion

        #region SnapLayout
        private const int HTMAXBUTTON = 9;
        private const string ButtonMax = "ButtonMax";
        private const string ButtonRestore = "ButtonRestore";
        private Button _ButtonMax;
        private Button _ButtonRestore;
        #endregion
    }
}
