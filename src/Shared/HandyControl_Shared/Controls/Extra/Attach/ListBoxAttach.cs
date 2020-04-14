using HandyControl.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace HandyControl.Controls
{
    public class ListBoxAttach
    {
        public static readonly DependencyProperty IsOddEvenRowProperty = DependencyProperty.RegisterAttached(
           "IsOddEvenRow", typeof(bool), typeof(ListBoxAttach), new FrameworkPropertyMetadata(ValueBoxes.FalseBox, FrameworkPropertyMetadataOptions.Inherits));

        public static void SetIsOddEvenRow(DependencyObject element, bool value) => element.SetValue(IsOddEvenRowProperty, value);

        public static bool GetIsOddEvenRow(DependencyObject element) => (bool)element.GetValue(IsOddEvenRowProperty);
    }
}
