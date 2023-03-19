using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace HandyControl.Themes
{
    public static class ThemeAnimationHelper
    {
        public async static void AnimateTheme(UIElement element, SlideDirection slideDirection, double durationSeconds, double fromOpacity, double toOpacity)
        {
            if (element != null)
            {
                DoubleAnimation opacityAnimation = new DoubleAnimation();
                opacityAnimation.From = fromOpacity;
                opacityAnimation.To = toOpacity;
                opacityAnimation.Duration = TimeSpan.FromSeconds(durationSeconds);

                ThicknessAnimation slideAnimation = new ThicknessAnimation();
                slideAnimation.Duration = TimeSpan.FromSeconds(durationSeconds);
                slideAnimation.From = GetSlideFromThickness(element, slideDirection);
                slideAnimation.To = new Thickness(0);

                Storyboard storyboard = new Storyboard();
                storyboard.Children.Add(opacityAnimation);
                storyboard.Children.Add(slideAnimation);
                Storyboard.SetTarget(opacityAnimation, element);
                Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(UIElement.OpacityProperty));
                Storyboard.SetTarget(slideAnimation, element);
                Storyboard.SetTargetProperty(slideAnimation, new PropertyPath(FrameworkElement.MarginProperty));

                storyboard.Begin();
                await Task.Delay(TimeSpan.FromSeconds(durationSeconds)); // Wait for the fade out animation to finish
            }
        }

        public enum SlideDirection
        {
            Left,
            Right,
            Top,
            Bottom
        }

        private static Thickness GetSlideFromThickness(UIElement element, SlideDirection slideDirection)
        {
            double elementWidth = element.RenderSize.Width;
            double elementHeight = element.RenderSize.Height;

            switch (slideDirection)
            {
                case SlideDirection.Left:
                    return new Thickness(-elementWidth, 0, elementWidth, 0);
                case SlideDirection.Right:
                    return new Thickness(elementWidth, 0, -elementWidth, 0);
                case SlideDirection.Top:
                    return new Thickness(0, -elementHeight, 0, elementHeight);
                case SlideDirection.Bottom:
                    return new Thickness(0, elementHeight, 0, -elementHeight);
                default:
                    return new Thickness();
            }
        }

    }
}
