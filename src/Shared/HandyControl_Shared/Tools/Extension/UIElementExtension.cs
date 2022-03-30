using System.Windows;

namespace HandyControl.Tools.Extension;

// ReSharper disable once InconsistentNaming
public static class UIElementExtension
{
    /// <summary>
    ///     Display element
    /// </summary>
    /// <param name="element"></param>
    public static void Show(this UIElement element) => element.Visibility = Visibility.Visible;

    /// <summary>
    ///     Display element
    /// </summary>
    /// <param name="element"></param>
    /// <param name="show"></param>
    public static void Show(this UIElement element, bool show) => element.Visibility = show ? Visibility.Visible : Visibility.Collapsed;

    /// <summary>
    ///     Unrealistic elements, but reserve space
    /// </summary>
    /// <param name="element"></param>
    public static void Hide(this UIElement element) => element.Visibility = Visibility.Hidden;

    /// <summary>
    ///     No elements are displayed, and no space is reserved
    /// </summary>
    /// <param name="element"></param>
    public static void Collapse(this UIElement element) => element.Visibility = Visibility.Collapsed;
}
