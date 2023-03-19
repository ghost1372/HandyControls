using System.Windows;
using HandyControl.Data;

namespace HandyControl.Controls;

public partial class MenuAttach
{
    public static readonly DependencyProperty AnimationModeProperty = DependencyProperty.RegisterAttached(
      "AnimationMode", typeof(TransitionMode), typeof(MenuAttach), new FrameworkPropertyMetadata(TransitionMode.Right2LeftWithFade, FrameworkPropertyMetadataOptions.Inherits, OnAnimationModeChanged));

    private static void OnAnimationModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TransitioningContentControl tc)
        {
            tc.Loaded -= Tc_Loaded;
            tc.Loaded += Tc_Loaded;
        }
    }

    private static void Tc_Loaded(object sender, RoutedEventArgs e)
    {
        var tc = sender as TransitioningContentControl;
        tc.StartTransition();
    }

    public static void SetAnimationMode(DependencyObject element, TransitionMode value)
        => element.SetValue(AnimationModeProperty, value);

    public static TransitionMode GetAnimationMode(DependencyObject element)
        => (TransitionMode) element.GetValue(AnimationModeProperty);

    public static readonly DependencyProperty IsEnabledAnimationProperty = DependencyProperty.RegisterAttached(
        "IsEnabledAnimation", typeof(bool), typeof(MenuAttach), new FrameworkPropertyMetadata(ValueBoxes.TrueBox, FrameworkPropertyMetadataOptions.Inherits));

    public static void SetIsEnabledAnimation(DependencyObject element, bool value)
        => element.SetValue(IsEnabledAnimationProperty, value);

    public static bool GetIsEnabledAnimation(DependencyObject element)
        => (bool) element.GetValue(IsEnabledAnimationProperty);
}
