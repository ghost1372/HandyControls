using System;
using System.Windows;

namespace HandyControl.Controls
{
    public abstract class DynamicLanguage
    {
        private static ResourceDictionary oldLanguage;

        /// <summary>
        /// Specify the folder and resourceDictionary file
        /// </summary>
        /// <param name="LanguageLocation">Sample: Language\English.xaml</param>
        public static void SetLanguageWithLocation(string LanguageLocation)
        {
            Uri LanguageUri = new Uri(LanguageLocation, UriKind.Relative);
            if (!string.IsNullOrEmpty(LanguageUri.OriginalString))
            {
                ResourceDictionary newLanguage = (ResourceDictionary)(Application.LoadComponent(LanguageUri));

                if (newLanguage != null)
                {
                    Application.Current.Resources.MergedDictionaries.Remove(oldLanguage);
                    oldLanguage = newLanguage;
                    Application.Current.Resources.MergedDictionaries.Add(newLanguage);
                }
            }
        }

        /// <summary>
        /// Resources Dictionary files must be placed under the name of Strings.{language code}.xaml and in the Assets folder
        /// <code>
        /// Folder Name: Assets
        /// <para>Dictionary Name: Strings.XX.xaml</para>
        /// <para>Sample: Assets\Strings.en.xaml</para>
        /// </code>
        /// </summary>
        /// <param name="lang">Language Code in 2-Digit format</param>
        public static void SetLanguage(string lang)
        {
            Uri LanguageUri = new Uri($@"Assets\Strings.{lang}.xaml", UriKind.Relative);
            if (!string.IsNullOrEmpty(LanguageUri.OriginalString))
            {
                ResourceDictionary newLanguage = (ResourceDictionary)(Application.LoadComponent(LanguageUri));

                if (newLanguage != null)
                {
                    Application.Current.Resources.MergedDictionaries.Remove(oldLanguage);
                    oldLanguage = newLanguage;
                    Application.Current.Resources.MergedDictionaries.Add(newLanguage);
                }
            }
        }

        /// <summary>
        /// Gets a resource as string
        /// </summary>
        /// <param name="key">The key of the string resource.</param>
        /// <param name="removeNewLines">If true, it removes any kind of new lines.</param>
        /// <param name="defaultValue">If the key is not found, the default value will be returned</param>
        /// <returns>A string resource, usually a localized string.</returns>
        public static string GetString(string key, string defaultValue = null, bool removeNewLines = false)
        {
            if (removeNewLines)
                return (Application.Current.TryFindResource(key) as string ?? "").Replace("\n", " ").Replace("\\n", " ").Replace("\r", " ").Replace("&#10;", " ").Replace("&#x0d;", " ");

            return Application.Current.TryFindResource(key) as string ?? defaultValue;
        }

        /// <summary>
        /// Gets a resource as string and applies the format
        /// </summary>
        /// <param name="key">The key of the string resource.</param>
        /// <param name="values">The values for the string format.</param>
        /// <returns>A string resource, usually a localized string.</returns>
        public static string GetStringFormat(string key, params object[] values)
        {
            return string.Format(Application.Current.TryFindResource(key) as string ?? "", values);
        }
    }
}
