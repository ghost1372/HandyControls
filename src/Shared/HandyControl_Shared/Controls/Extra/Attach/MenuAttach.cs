using System.Windows;
using HandyControl.Data;

namespace HandyControl.Controls
{
    public class MenuAttach
    {
        public static readonly DependencyProperty AnimationModeProperty = DependencyProperty.RegisterAttached(
          "AnimationMode", typeof(TransitionMode), typeof(MenuAttach), new FrameworkPropertyMetadata(TransitionMode.Right2LeftWithFade, FrameworkPropertyMetadataOptions.Inherits));

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
}
