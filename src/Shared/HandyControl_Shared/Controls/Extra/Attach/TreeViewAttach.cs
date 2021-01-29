using HandyControl.Data;
using System.Windows;

namespace HandyControl.Controls
{
    internal class TreeViewAttach
    {
        internal static readonly DependencyProperty IsCheckTreeViewProperty = DependencyProperty.RegisterAttached(
           "IsCheckTreeView", typeof(bool), typeof(TreeViewAttach), new FrameworkPropertyMetadata(ValueBoxes.FalseBox, FrameworkPropertyMetadataOptions.Inherits));

        public static void SetIsCheckTreeView(DependencyObject element, bool value) => element.SetValue(IsCheckTreeViewProperty, value);

        public static bool GetIsCheckTreeView(DependencyObject element) => (bool)element.GetValue(IsCheckTreeViewProperty);
    }
}
