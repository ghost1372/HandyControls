using System.Windows;

namespace HandyControl.Tools;

public class BindingProxy : Freezable
{
    public object Value
    {
        get { return (object) GetValue(ValueProperty); }
        set { SetValue(ValueProperty, value); }
    }

    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register("Value", typeof(object), typeof(BindingProxy));

    protected override Freezable CreateInstanceCore()
    {
        return new BindingProxy();
    }
}
