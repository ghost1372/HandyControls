using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using HandyControl.Data;
using HandyControl.Tools;
using HandyControl.Tools.Interop;

namespace HandyControl.Controls;

public partial class Window
{
    #region Title

    public static readonly DependencyProperty TitleAlignmentProperty = DependencyProperty.Register(
        "TitleAlignment", typeof(HorizontalAlignment), typeof(Window),
        new PropertyMetadata(HorizontalAlignment.Left));

    public HorizontalAlignment TitleAlignment
    {
        get => (HorizontalAlignment) GetValue(TitleAlignmentProperty);
        set => SetValue(TitleAlignmentProperty, (HorizontalAlignment) value);
    }

    public static readonly DependencyProperty TitleMarginProperty = DependencyProperty.Register(
        "TitleMargin", typeof(Thickness), typeof(Window),
        new PropertyMetadata(new Thickness(0)));

    public Thickness TitleMargin
    {
        get => (Thickness) GetValue(TitleMarginProperty);
        set => SetValue(TitleMarginProperty, (Thickness) value);
    }

    #endregion

    #region Mica

    public static readonly DependencyProperty SystemBackdropTypeProperty = DependencyProperty.Register(
        "SystemBackdropType", typeof(BackdropType), typeof(Window),
        new PropertyMetadata(BackdropType.Auto, OnSystemBackdropTypeChanged));

    public BackdropType SystemBackdropType
    {
        get => (BackdropType) GetValue(SystemBackdropTypeProperty);
        set => SetValue(SystemBackdropTypeProperty, (BackdropType) value);
    }

    private static void OnSystemBackdropTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (Window) d;
        ctl.InitMica((Window) d, (BackdropType) e.NewValue);
    }

    private void InitMica(Window window, BackdropType backdropType)
    {
        if (backdropType == BackdropType.Disable)
        {
            SetResourceReference(NonClientAreaBackgroundProperty, "RegionBrush");
        }
        else
        {
            NonClientAreaBackground = Brushes.Transparent;
        }

        Background = Brushes.Transparent;
        window.Apply(backdropType);
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

    private const int GW_HWNDNEXT = 2;
    public void FixCut()
    {
        if (WindowState == WindowState.Maximized)
        {
            IntPtr hWnd = new WindowInteropHelper(Application.Current.MainWindow).Handle;

            IntPtr hNext = hWnd;
            do
                hNext = InteropMethods.GetWindow(hNext, GW_HWNDNEXT);
            while (!InteropMethods.IsWindowVisible(hNext));

            InteropMethods.SetForegroundWindow(hNext);

            Activate();
        }
    }

    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (MicaHelper.IsMica)
        {
            FixCut();
        }
    }
}
