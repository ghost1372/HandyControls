// http://github.com/kinnara/ModernWpf

using System.Windows;
using HandyControl.Tools;

namespace HandyControl.Themes
{
    /// <summary>
    /// Default styles for controls.
    /// </summary>
    public class Theme : ResourceDictionary
    {
        /// <summary>
        /// Initializes a new instance of the Theme class.
        /// </summary>
        public Theme()
        {
            MergedDictionaries.Add(ControlsResources);
        }

        internal static ResourceDictionary ControlsResources
        {
            get
            {
                if (_controlsResources == null)
                {
                    _controlsResources = new ResourceDictionary { Source = ApplicationHelper.GetAbsoluteUri("Themes/Theme.xaml") };
                }
                return _controlsResources;
            }
        }

        private static ResourceDictionary _controlsResources;
    }
}
