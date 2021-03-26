using System.Windows;
using HandyControl.Data;

namespace HandyControl.Controls
{
    /// <summary>
    ///     Rule ItemsControl
    /// </summary>
    /// <remarks>
    ///     Each item in this category has the same size and margin
    /// </remarks>
    public class RegularItemsControl : SimpleItemsControl
    {
        public static readonly DependencyProperty ItemWidthProperty = DependencyProperty.Register(
            "ItemWidth", typeof(double), typeof(RegularItemsControl), new PropertyMetadata(ValueBoxes.Double200Box));

        public double ItemWidth
        {
            get => (double) GetValue(ItemWidthProperty);
            set => SetValue(ItemWidthProperty, value);
        }

        public static readonly DependencyProperty ItemHeightProperty = DependencyProperty.Register(
            "ItemHeight", typeof(double), typeof(RegularItemsControl), new PropertyMetadata(ValueBoxes.Double200Box));

        public double ItemHeight
        {
            get => (double) GetValue(ItemHeightProperty);
            set => SetValue(ItemHeightProperty, value);
        }

        public static readonly DependencyProperty ItemMarginProperty = DependencyProperty.Register(
            "ItemMargin", typeof(Thickness), typeof(RegularItemsControl), new PropertyMetadata(default(Thickness)));

        public Thickness ItemMargin
        {
            get => (Thickness) GetValue(ItemMarginProperty);
            set => SetValue(ItemMarginProperty, value);
        }
    }
}
