using System.Windows;
using System.Windows.Controls;

namespace HandyControl.Controls
{
    public class MainContentBackground : ContentControl
    {
        static MainContentBackground()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MainContentBackground),
                   new FrameworkPropertyMetadata(typeof(MainContentBackground)));
        }
    }
}
