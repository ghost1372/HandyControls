﻿using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using HandyControl.Data;
using HandyControl.Tools;
using HandyControl.Tools.Extension;
using HandyControl.Tools.Interop;
using System.Windows.Controls;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
#if NET40
using Microsoft.Windows.Shell;
#else
using System.Windows.Shell;
#endif

namespace HandyControl.Controls
{
    [TemplatePart(Name = ElementNonClientArea, Type = typeof(UIElement))]
    public partial class Window : System.Windows.Window
    {
        #region fields

        private const string ElementNonClientArea = "PART_NonClientArea";

        private bool _isFullScreen;

        private Thickness _actualBorderThickness;

        private readonly Thickness _commonPadding;

        private bool _showNonClientArea = true;

        private double _tempNonClientAreaHeight;

        private WindowState _tempWindowState;

        private WindowStyle _tempWindowStyle;

        private ResizeMode _tempResizeMode;

        private UIElement _nonClientArea;

        #endregion

        #region ctor

        static Window()
        {
            StyleProperty.OverrideMetadata(typeof(Window), new FrameworkPropertyMetadata(ResourceHelper.GetResourceInternal<Style>(ResourceToken.WindowWin10)));
        }

        public Window()
        {
#if NET40
            var chrome = new WindowChrome
            {
                CornerRadius = new CornerRadius(),
                GlassFrameThickness = new Thickness(0, 0, 0, 1)
            };
#else
            var chrome = new WindowChrome
            {
                CornerRadius = new CornerRadius(),
                ResizeBorderThickness = new Thickness(8),
                GlassFrameThickness = new Thickness(0, 0, 0, 1),
                UseAeroCaptionButtons = false
            };
#endif
            BindingOperations.SetBinding(chrome, WindowChrome.CaptionHeightProperty,
                new Binding(NonClientAreaHeightProperty.Name) { Source = this });
            WindowChrome.SetWindowChrome(this, chrome);
            _commonPadding = Padding;

            Loaded += (s, e) => OnLoaded(e);
        }

        #endregion 

        #region prop

        public static readonly DependencyProperty NonClientAreaContentProperty = DependencyProperty.Register(
            nameof(NonClientAreaContent), typeof(object), typeof(Window), new PropertyMetadata(default(object)));

        public object NonClientAreaContent
        {
            get => GetValue(NonClientAreaContentProperty);
            set => SetValue(NonClientAreaContentProperty, value);
        }

        public static readonly DependencyProperty CloseButtonHoverBackgroundProperty = DependencyProperty.Register(
            nameof(CloseButtonHoverBackground), typeof(Brush), typeof(Window),
            new PropertyMetadata(default(Brush)));

        public Brush CloseButtonHoverBackground
        {
            get => (Brush) GetValue(CloseButtonHoverBackgroundProperty);
            set => SetValue(CloseButtonHoverBackgroundProperty, value);
        }

        public static readonly DependencyProperty CloseButtonHoverForegroundProperty =
            DependencyProperty.Register(
                nameof(CloseButtonHoverForeground), typeof(Brush), typeof(Window),
                new PropertyMetadata(default(Brush)));

        public Brush CloseButtonHoverForeground
        {
            get => (Brush) GetValue(CloseButtonHoverForegroundProperty);
            set => SetValue(CloseButtonHoverForegroundProperty, value);
        }

        public static readonly DependencyProperty CloseButtonBackgroundProperty = DependencyProperty.Register(
            nameof(CloseButtonBackground), typeof(Brush), typeof(Window), new PropertyMetadata(Brushes.Transparent));

        public Brush CloseButtonBackground
        {
            get => (Brush) GetValue(CloseButtonBackgroundProperty);
            set => SetValue(CloseButtonBackgroundProperty, value);
        }

        public static readonly DependencyProperty CloseButtonForegroundProperty = DependencyProperty.Register(
            nameof(CloseButtonForeground), typeof(Brush), typeof(Window),
            new PropertyMetadata(Brushes.White));

        public Brush CloseButtonForeground
        {
            get => (Brush) GetValue(CloseButtonForegroundProperty);
            set => SetValue(CloseButtonForegroundProperty, value);
        }

        public static readonly DependencyProperty OtherButtonBackgroundProperty = DependencyProperty.Register(
            nameof(OtherButtonBackground), typeof(Brush), typeof(Window), new PropertyMetadata(Brushes.Transparent));

        public Brush OtherButtonBackground
        {
            get => (Brush) GetValue(OtherButtonBackgroundProperty);
            set => SetValue(OtherButtonBackgroundProperty, value);
        }

        public static readonly DependencyProperty OtherButtonForegroundProperty = DependencyProperty.Register(
            nameof(OtherButtonForeground), typeof(Brush), typeof(Window),
            new PropertyMetadata(Brushes.White));

        public Brush OtherButtonForeground
        {
            get => (Brush) GetValue(OtherButtonForegroundProperty);
            set => SetValue(OtherButtonForegroundProperty, value);
        }

        public static readonly DependencyProperty OtherButtonHoverBackgroundProperty = DependencyProperty.Register(
            nameof(OtherButtonHoverBackground), typeof(Brush), typeof(Window),
            new PropertyMetadata(default(Brush)));

        public Brush OtherButtonHoverBackground
        {
            get => (Brush) GetValue(OtherButtonHoverBackgroundProperty);
            set => SetValue(OtherButtonHoverBackgroundProperty, value);
        }

        public static readonly DependencyProperty OtherButtonHoverForegroundProperty =
            DependencyProperty.Register(
                nameof(OtherButtonHoverForeground), typeof(Brush), typeof(Window),
                new PropertyMetadata(default(Brush)));

        public Brush OtherButtonHoverForeground
        {
            get => (Brush) GetValue(OtherButtonHoverForegroundProperty);
            set => SetValue(OtherButtonHoverForegroundProperty, value);
        }

        public static readonly DependencyProperty NonClientAreaBackgroundProperty = DependencyProperty.Register(
            nameof(NonClientAreaBackground), typeof(Brush), typeof(Window),
            new PropertyMetadata(default(Brush)));

        public Brush NonClientAreaBackground
        {
            get => (Brush) GetValue(NonClientAreaBackgroundProperty);
            set => SetValue(NonClientAreaBackgroundProperty, value);
        }

        public static readonly DependencyProperty NonClientAreaForegroundProperty = DependencyProperty.Register(
            nameof(NonClientAreaForeground), typeof(Brush), typeof(Window),
            new PropertyMetadata(default(Brush)));

        public Brush NonClientAreaForeground
        {
            get => (Brush) GetValue(NonClientAreaForegroundProperty);
            set => SetValue(NonClientAreaForegroundProperty, value);
        }

        public static readonly DependencyProperty NonClientAreaHeightProperty = DependencyProperty.Register(
            nameof(NonClientAreaHeight), typeof(double), typeof(Window),
            new PropertyMetadata(22.0));

        public double NonClientAreaHeight
        {
            get => (double) GetValue(NonClientAreaHeightProperty);
            set => SetValue(NonClientAreaHeightProperty, value);
        }

        public static readonly DependencyProperty ShowNonClientAreaProperty = DependencyProperty.Register(
            nameof(ShowNonClientArea), typeof(bool), typeof(Window),
            new PropertyMetadata(ValueBoxes.TrueBox, OnShowNonClientAreaChanged));

        public bool ShowNonClientArea
        {
            get => (bool) GetValue(ShowNonClientAreaProperty);
            set => SetValue(ShowNonClientAreaProperty, ValueBoxes.BooleanBox(value));
        }

        public static readonly DependencyProperty ShowTitleProperty = DependencyProperty.Register(
            nameof(ShowTitle), typeof(bool), typeof(Window), new PropertyMetadata(ValueBoxes.TrueBox));

        public bool ShowTitle
        {
            get => (bool) GetValue(ShowTitleProperty);
            set => SetValue(ShowTitleProperty, ValueBoxes.BooleanBox(value));
        }

        public static readonly DependencyProperty IsFullScreenProperty = DependencyProperty.Register(
            nameof(IsFullScreen), typeof(bool), typeof(Window),
            new PropertyMetadata(ValueBoxes.FalseBox, OnIsFullScreenChanged));

        public bool IsFullScreen
        {
            get => (bool) GetValue(IsFullScreenProperty);
            set => SetValue(IsFullScreenProperty, ValueBoxes.BooleanBox(value));
        }

        public static readonly DependencyProperty ShowIconProperty = DependencyProperty.Register(
            nameof(ShowIcon), typeof(bool), typeof(Window), new PropertyMetadata(ValueBoxes.TrueBox));

        public bool ShowIcon
        {
            get => (bool) GetValue(ShowIconProperty);
            set => SetValue(ShowIconProperty, value);
        }

        #endregion

        #region methods

        #region public

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _nonClientArea = GetTemplateChild(ElementNonClientArea) as UIElement;
            _ButtonMax = GetTemplateChild(ButtonMax) as Button;
            _ButtonRestore = GetTemplateChild(ButtonRestore) as Button;
        }

        #endregion

        #region protected

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            this.GetHwndSource()?.AddHook(HwndSourceHook);
        }

        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);
            if (WindowState == WindowState.Maximized)
            {
                BorderThickness = new Thickness();
                _tempNonClientAreaHeight = NonClientAreaHeight;
                NonClientAreaHeight += 8;
            }
            else
            {
                BorderThickness = _actualBorderThickness;
                NonClientAreaHeight = _tempNonClientAreaHeight;
            }

            if (MicaHelper.IsMica)
            {
                FixCut();
            }
        }

        protected void OnLoaded(RoutedEventArgs args)
        {
            _actualBorderThickness = BorderThickness;
            _tempNonClientAreaHeight = NonClientAreaHeight;

            if (WindowState == WindowState.Maximized)
            {
                BorderThickness = new Thickness();
                _tempNonClientAreaHeight += 8;
            }

            CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand,
                (s, e) => WindowState = WindowState.Minimized));
            CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand,
                (s, e) => WindowState = WindowState.Maximized));
            CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand,
                (s, e) => WindowState = WindowState.Normal));
            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, (s, e) => Close()));
            CommandBindings.Add(new CommandBinding(SystemCommands.ShowSystemMenuCommand, ShowSystemMenu));

            _tempWindowState = WindowState;
            _tempWindowStyle = WindowStyle;
            _tempResizeMode = ResizeMode;

            if (ResizeMode == ResizeMode.NoResize || WindowState == WindowState.Maximized)
            {
                var chrome = WindowChrome.GetWindowChrome(this);
                chrome.ResizeBorderThickness = new Thickness(0);
                WindowChrome.SetWindowChrome(this, chrome);
            }

            SwitchIsFullScreen(_isFullScreen);
            SwitchShowNonClientArea(_showNonClientArea);

            if (WindowState == WindowState.Maximized)
            {
                _tempNonClientAreaHeight -= 8;
            }

            if (SizeToContent != SizeToContent.WidthAndHeight)
                return;

            SizeToContent = SizeToContent.Height;
            Dispatcher.BeginInvoke(new Action(() => { SizeToContent = SizeToContent.WidthAndHeight; }));
            SizeChanged += Window_SizeChanged;
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            if (SizeToContent == SizeToContent.WidthAndHeight)
                InvalidateMeasure();
        }

        #endregion

        #region private

        private void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
        {
            var mmi = (InteropValues.MINMAXINFO) Marshal.PtrToStructure(lParam, typeof(InteropValues.MINMAXINFO));
            var monitor = InteropMethods.MonitorFromWindow(hwnd, InteropValues.MONITOR_DEFAULTTONEAREST);

            if (monitor != IntPtr.Zero && mmi != null)
            {
                InteropValues.APPBARDATA appBarData = default;
                var autoHide = InteropMethods.SHAppBarMessage(4, ref appBarData) != 0;
                if (autoHide)
                {
                    var monitorInfo = default(InteropValues.MONITORINFO);
                    monitorInfo.cbSize = (uint) Marshal.SizeOf(typeof(InteropValues.MONITORINFO));
                    InteropMethods.GetMonitorInfo(monitor, ref monitorInfo);
                    var rcWorkArea = monitorInfo.rcWork;
                    var rcMonitorArea = monitorInfo.rcMonitor;
                    mmi.ptMaxPosition.X = Math.Abs(rcWorkArea.Left - rcMonitorArea.Left);
                    mmi.ptMaxPosition.Y = Math.Abs(rcWorkArea.Top - rcMonitorArea.Top);
                    mmi.ptMaxSize.X = Math.Abs(rcWorkArea.Right - rcWorkArea.Left);
                    mmi.ptMaxSize.Y = Math.Abs(rcWorkArea.Bottom - rcWorkArea.Top - 1);
                }
            }

            Marshal.StructureToPtr(mmi, lParam, true);
        }

        private IntPtr HwndSourceHook(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
        {
            switch (msg)
            {
                case InteropValues.WM_WINDOWPOSCHANGED:
                    Padding = WindowState == WindowState.Maximized ? WindowHelper.WindowMaximizedPadding : _commonPadding;
                    break;
                case InteropValues.WM_GETMINMAXINFO:
                    WmGetMinMaxInfo(hwnd, lparam);
                    Padding = WindowState == WindowState.Maximized ? WindowHelper.WindowMaximizedPadding : _commonPadding;
                    break;
                #region SnapLayout
                case InteropValues.WM_NCHITTEST:
                    try
                    {
                        int x = lparam.ToInt32() & 0xffff;
                        if (OSVersionHelper.IsWindows11_OrGreater && ShowNonClientArea && ShowMaxButton && ResizeMode is not ResizeMode.NoResize and not ResizeMode.CanMinimize)
                        {
                            int y = lparam.ToInt32() >> 16;
                            var DPI_SCALE = DpiHelper.LogicalToDeviceUnitsScalingFactorX;
                            var _button = WindowState == WindowState.Maximized ? _ButtonRestore : _ButtonMax;
                            if (_button != null)
                            {
                                var rect = new Rect(_button.PointToScreen(
                                new Point()),
                                new Size(_button.Width * DPI_SCALE, _button.Height * DPI_SCALE));
                                if (rect.Contains(new Point(x, y)))
                                {
                                    handled = true;
                                    _button.Background = OtherButtonHoverBackground;
                                }
                                else
                                {
                                    _button.Background = OtherButtonBackground;
                                }
                                return new IntPtr(HTMAXBUTTON);
                            }
                        }
                    }
                    catch (OverflowException)
                    {
                        handled = true;
                    }
                    break;
                case InteropValues.WM_NCLBUTTONDOWN:
                    if (OSVersionHelper.IsWindows11_OrGreater && ShowNonClientArea && ShowMaxButton && ResizeMode is not ResizeMode.NoResize and not ResizeMode.CanMinimize)
                    {
                        int x = lparam.ToInt32() & 0xffff;
                        int y = lparam.ToInt32() >> 16;
                        var DPI_SCALE = DpiHelper.LogicalToDeviceUnitsScalingFactorX;
                        var _button = WindowState == WindowState.Maximized ? _ButtonRestore : _ButtonMax;
                        if (_button != null)
                        {
                            var rect = new Rect(_button.PointToScreen(
                            new Point()),
                            new Size(_button.Width * DPI_SCALE, _button.Height * DPI_SCALE));
                            if (rect.Contains(new Point(x, y)))
                            {
                                handled = true;
                                IInvokeProvider invokeProv = new ButtonAutomationPeer(_button).GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                                invokeProv?.Invoke();
                            }
                        }
                    }
                    break;
                #endregion
                #region System Command
                case InteropValues.WM_SYSCOMMAND:
                    if (!ShowMaxButton)
                    {
                        if ((int) wparam == InteropValues.SC_MAXIMIZE)
                        {
                            handled = true;
                        }
                    }
                    if (!ShowMinButton)
                    {
                        if ((int) wparam == InteropValues.SC_MINIMIZE)
                        {
                            handled = true;
                        }
                    }
                    if (!ShowCloseButton)
                    {
                        if ((int) wparam == InteropValues.SC_CLOSE)
                        {
                            handled = true;
                        }
                    }
                    break;
                case InteropValues.WM_NCLBUTTONDBLCLK:
                    if (!ShowMaxButton)
                    {
                        handled = true;
                    }
                    break;
                #endregion
                default:
                    handled = false;
                    break;
            }

            return IntPtr.Zero;
        }

        private static void OnShowNonClientAreaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctl = (Window) d;
            ctl.SwitchShowNonClientArea((bool) e.NewValue);
        }

        private void SwitchShowNonClientArea(bool showNonClientArea)
        {
            if (_nonClientArea == null)
            {
                _showNonClientArea = showNonClientArea;
                return;
            }

            if (showNonClientArea)
            {
                if (IsFullScreen)
                {
                    _nonClientArea.Show(false);
                    _tempNonClientAreaHeight = NonClientAreaHeight;
                    NonClientAreaHeight = 0;
                }
                else
                {
                    _nonClientArea.Show(true);
                    NonClientAreaHeight = _tempNonClientAreaHeight;
                }
            }
            else
            {
                _nonClientArea.Show(false);
                _tempNonClientAreaHeight = NonClientAreaHeight;
                NonClientAreaHeight = 0;
            }
        }

        private static void OnIsFullScreenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctl = (Window) d;
            ctl.SwitchIsFullScreen((bool) e.NewValue);
        }

        private void SwitchIsFullScreen(bool isFullScreen)
        {
            if (_nonClientArea == null)
            {
                _isFullScreen = isFullScreen;
                return;
            }

            if (isFullScreen)
            {
                _nonClientArea.Show(false);
                _tempNonClientAreaHeight = NonClientAreaHeight;
                NonClientAreaHeight = 0;

                _tempWindowState = WindowState;
                _tempWindowStyle = WindowStyle;
                _tempResizeMode = ResizeMode;
                WindowStyle = WindowStyle.None;
                //下面三行不能改变，就是故意的
                WindowState = WindowState.Maximized;
                WindowState = WindowState.Minimized;
                WindowState = WindowState.Maximized;
            }
            else
            {
                if (ShowNonClientArea)
                {
                    _nonClientArea.Show(true);
                    NonClientAreaHeight = _tempNonClientAreaHeight;
                }
                else
                {
                    _nonClientArea.Show(false);
                    _tempNonClientAreaHeight = NonClientAreaHeight;
                    NonClientAreaHeight = 0;
                }

                WindowState = _tempWindowState;
                WindowStyle = _tempWindowStyle;
                ResizeMode = _tempResizeMode;
            }
        }

        private void ShowSystemMenu(object sender, ExecutedRoutedEventArgs e)
        {
            var point = WindowState == WindowState.Maximized
                ? new Point(0, NonClientAreaHeight)
                : new Point(Left, Top + NonClientAreaHeight);
            SystemCommands.ShowSystemMenu(this, point);
        }

        #endregion

        #endregion
    }
}
