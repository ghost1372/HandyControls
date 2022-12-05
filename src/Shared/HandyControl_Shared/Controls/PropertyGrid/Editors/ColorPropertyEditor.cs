using System.Windows;

namespace HandyControl.Controls
{
    public class ColorPropertyEditor : PropertyEditorBase
    {
        public override FrameworkElement CreateElement(PropertyItem propertyItem) => new ColorPickerBox
        {
            IsEnabled = !propertyItem.IsReadOnly
        };

        public override DependencyProperty GetDependencyProperty() => ColorPickerBox.SelectedBrushProperty;
    }
}
