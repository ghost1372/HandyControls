using System.Windows;
using HandyControl.Data;

namespace HandyControl.Controls;

public class StatusSwitchElement
{
    /// <summary>
    ///     Elements to be displayed when selected
    /// </summary>
    public static readonly DependencyProperty CheckedElementProperty = DependencyProperty.RegisterAttached(
        "CheckedElement", typeof(object), typeof(StatusSwitchElement), new PropertyMetadata(default(object)));

    public static void SetCheckedElement(DependencyObject element, object value) => element.SetValue(CheckedElementProperty, value);

    public static object GetCheckedElement(DependencyObject element) => element.GetValue(CheckedElementProperty);

    /// <summary>
    ///     Whether to hide the element
    /// </summary>
    public static readonly DependencyProperty HideUncheckedElementProperty = DependencyProperty.RegisterAttached(
        "HideUncheckedElement", typeof(bool), typeof(StatusSwitchElement), new PropertyMetadata(ValueBoxes.FalseBox));

    public static void SetHideUncheckedElement(DependencyObject element, bool value) => element.SetValue(HideUncheckedElementProperty, ValueBoxes.BooleanBox(value));

    public static bool GetHideUncheckedElement(DependencyObject element) => (bool) element.GetValue(HideUncheckedElementProperty);
}
