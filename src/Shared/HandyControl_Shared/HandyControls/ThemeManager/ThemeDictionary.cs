// http://github.com/kinnara/ModernWpf

using System.Windows;

namespace HandyControl.Themes
{
    public static class ThemeDictionary
    {
        public static void SetKey(ResourceDictionary themeDictionary, string key)
        {
            var baseThemeDictionary = GetBaseThemeDictionary(key);
            themeDictionary.MergedDictionaries.Insert(0, baseThemeDictionary);
        }

        private static ResourceDictionary GetBaseThemeDictionary(string key)
        {
            ResourceDictionary themeDictionary = ThemeResources.Current?.TryGetThemeDictionary(key);
            return themeDictionary ?? ThemeManager.GetDefaultThemeDictionary(key);
        }
    }
}
