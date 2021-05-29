using System;
using System.Windows;

namespace HandyControl.Tools
{
    /// <summary>
    ///     Resource help class
    /// </summary>
    public class ResourceHelper
    {
        /// <summary>
        ///     Get Resource
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetResource<T>(string key)
        {
            if (Application.Current.TryFindResource(key) is T resource)
            {
                return resource;
            }

            return default;
        }

        internal static T GetResourceInternal<T>(string key)
        {
            if (GetTheme()[key] is T resource)
            {
                return resource;
            }

            return default;
        }

        public static ResourceDictionary GetTheme() => new Lazy<ResourceDictionary>(() => new ResourceDictionary
        {
            Source = ApplicationHelper.GetAbsoluteUri("Themes/Theme.xaml")
        }).Value;
    }
}
