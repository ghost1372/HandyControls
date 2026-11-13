using System.Windows;

namespace HandyControl.Controls;

/// <summary>
/// Header text orientation of an <see cref="Expander"/> when <see cref="Expander.ExpandDirection"/> is Left or Right.
/// </summary>
public enum ExpanderHeaderTextOrientation
{
    /// <summary>
    /// Same as the current behavior: the whole header is rotated -90 degrees (glyphs lying on their side).
    /// </summary>
    Default = 0,

    /// <summary>
    /// Upright vertical: characters stand upright and stack top-to-bottom.
    /// </summary>
    Vertical = 1,

    /// <summary>
    /// Upright horizontal: a single horizontal line.
    /// </summary>
    Horizontal = 2
}

/// <summary>
/// Attached properties for <see cref="Expander"/>.
/// </summary>
public class ExpanderElement
{
    public static readonly DependencyProperty HeaderTextOrientationProperty = DependencyProperty.RegisterAttached(
        "HeaderTextOrientation", typeof(ExpanderHeaderTextOrientation), typeof(ExpanderElement), new FrameworkPropertyMetadata(ExpanderHeaderTextOrientation.Default));

    public static void SetHeaderTextOrientation(DependencyObject element, ExpanderHeaderTextOrientation value) => element.SetValue(HeaderTextOrientationProperty, value);

    public static ExpanderHeaderTextOrientation GetHeaderTextOrientation(DependencyObject element) => (ExpanderHeaderTextOrientation) element.GetValue(HeaderTextOrientationProperty);
}
