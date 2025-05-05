using System.Windows;

namespace HandyControl.Controls;

public partial class Carousel
{
    public Style PreviousButtonStyle
    {
        get { return (Style) GetValue(PreviousButtonStyleProperty); }
        set { SetValue(PreviousButtonStyleProperty, value); }
    }

    public static readonly DependencyProperty PreviousButtonStyleProperty =
        DependencyProperty.Register(nameof(PreviousButtonStyle), typeof(Style), typeof(Carousel), new PropertyMetadata(Application.Current.Resources["ButtonCustom"] as Style));

    public Style NextButtonStyle
    {
        get { return (Style) GetValue(NextButtonStyleProperty); }
        set { SetValue(NextButtonStyleProperty, value); }
    }

    public static readonly DependencyProperty NextButtonStyleProperty =
        DependencyProperty.Register(nameof(NextButtonStyle), typeof(Style), typeof(Carousel), new PropertyMetadata(Application.Current.Resources["ButtonCustom"] as Style));
}
