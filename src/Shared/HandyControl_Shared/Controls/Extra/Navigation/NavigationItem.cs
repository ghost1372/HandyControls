using System.Windows;
namespace HandyControl.Controls
{
    public class NavigationItem : TabItem
    {
        public static readonly DependencyProperty TextHorizontalAlignmentProperty = ElementBase.Property<NavigationItem, HorizontalAlignment>(nameof(TextHorizontalAlignmentProperty), HorizontalAlignment.Right);

        public HorizontalAlignment TextHorizontalAlignment { get { return (HorizontalAlignment)GetValue(TextHorizontalAlignmentProperty); } set { SetValue(TextHorizontalAlignmentProperty, value); } }

        static NavigationItem()
        {
            ElementBase.DefaultStyle<NavigationItem>(DefaultStyleKeyProperty);
        }
    }
}
