using System.ComponentModel;
using System.Globalization;

namespace HandyControl.Tools
{
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ILocalizationProviderExtensions
    {
        public static string Localize(this ILocalizationProvider provider, string key, CultureInfo culture, string fallbackValue, string stringFormat)
        {
            if (string.IsNullOrEmpty(stringFormat))
            {
                return provider.Localize(key, culture, fallbackValue);
            }
            else
            {
                return string.Format(stringFormat, provider.Localize(key, culture, fallbackValue));
            }
        }

        public static string Localize(this ILocalizationProvider provider, string key, CultureInfo culture, string fallbackValue)
        {
            return provider.Localize(key, culture) ?? fallbackValue;
        }
    }
}
