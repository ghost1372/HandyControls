using System.Windows;

namespace HandyControl.Controls;

public partial class CoverView
{
    public bool ShowContent
    {
        get { return (bool) GetValue(ShowContentProperty); }
        set { SetValue(ShowContentProperty, value); }
    }

    public static readonly DependencyProperty ShowContentProperty =
        DependencyProperty.Register("ShowContent", typeof(bool), typeof(CoverView), new PropertyMetadata(true));
}
