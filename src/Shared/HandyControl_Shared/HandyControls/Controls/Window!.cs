using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HandyControl.Data;
using HandyControl.Tools;
#if NET40
using Microsoft.Windows.Shell;
#else
using System.Windows.Shell;
#endif
namespace HandyControl.Controls;

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
#if NET40
            var chrome = new WindowChrome
            {
                CornerRadius = new CornerRadius(),
                GlassFrameThickness = new Thickness(-1),
                ResizeBorderThickness = new Thickness(8)
            };
#else
            var chrome = new WindowChrome
            {
                CornerRadius = new CornerRadius(),
                ResizeBorderThickness = new Thickness(8),
                GlassFrameThickness = new Thickness(-1),
                NonClientFrameEdges = NonClientFrameEdges.None,
                UseAeroCaptionButtons = false
            };
#endif
            WindowChrome.SetWindowChrome(this, chrome);
            NonClientAreaBackground = Brushes.Transparent;
            MicaHelper.ApplyMicaEffect(this);
        }
        else
        {
            MicaHelper.RemoveMicaEffect();
        }
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
