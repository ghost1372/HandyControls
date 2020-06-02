using HandyControl.Data;
using System.Windows;
using System.Windows.Media;

namespace HandyControl.Controls
{
    public class ListBoxAttach
    {
        public static readonly DependencyProperty IsOddEvenRowProperty = DependencyProperty.RegisterAttached(
          "IsOddEvenRow", typeof(bool), typeof(ListBoxAttach), new FrameworkPropertyMetadata(ValueBoxes.FalseBox, FrameworkPropertyMetadataOptions.Inherits));

        public static void SetIsOddEvenRow(DependencyObject element, bool value)
            => element.SetValue(IsOddEvenRowProperty, value);

        public static bool GetIsOddEvenRow(DependencyObject element)
            => (bool)element.GetValue(IsOddEvenRowProperty);


        public static readonly DependencyProperty IsNewProperty = DependencyProperty.RegisterAttached(
          "IsNew", typeof(bool), typeof(ListBoxAttach), new FrameworkPropertyMetadata(ValueBoxes.FalseBox,
              FrameworkPropertyMetadataOptions.Inherits));

        public static void SetIsNew(DependencyObject element, bool value)
            => element.SetValue(IsNewProperty, value);

        public static bool GetIsNew(DependencyObject element)
            => (bool)element.GetValue(IsNewProperty);

        public static readonly DependencyProperty GeometryProperty = DependencyProperty.RegisterAttached(
          "Geometry", typeof(Geometry), typeof(ListBoxAttach), new PropertyMetadata(default(Geometry)));

        public static void SetGeometry(DependencyObject element, Geometry value)
            => element.SetValue(GeometryProperty, value);

        public static Geometry GetGeometry(DependencyObject element)
            => (Geometry)element.GetValue(GeometryProperty);

        public static readonly DependencyProperty WidthProperty = DependencyProperty.RegisterAttached(
          "Width", typeof(double), typeof(ListBoxAttach), new PropertyMetadata(double.NaN));

        public static void SetWidth(DependencyObject element, double value)
            => element.SetValue(WidthProperty, value);

        public static double GetWidth(DependencyObject element)
            => (double)element.GetValue(WidthProperty);

        public static readonly DependencyProperty HeightProperty = DependencyProperty.RegisterAttached(
          "Height", typeof(double), typeof(ListBoxAttach), new PropertyMetadata(double.NaN));

        public static void SetHeight(DependencyObject element, double value)
            => element.SetValue(HeightProperty, value);

        public static double GetHeight(DependencyObject element)
            => (double)element.GetValue(HeightProperty);

        public static readonly DependencyProperty GeoemtryBrushProperty = DependencyProperty.RegisterAttached(
          "GeoemtryBrush", typeof(Brush), typeof(ListBoxAttach), new FrameworkPropertyMetadata(default(Brushes), FrameworkPropertyMetadataOptions.Inherits));

        public static void SetGeoemtryBrush(DependencyObject element, Brush value)
            => element.SetValue(GeoemtryBrushProperty, value);

        public static Brush GetGeoemtryBrush(DependencyObject element)
            => (Brush)element.GetValue(GeoemtryBrushProperty);
    }
}
