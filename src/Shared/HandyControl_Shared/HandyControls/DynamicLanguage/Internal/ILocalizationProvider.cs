// https://github.com/MartinKuschnik/Goji

using System.ComponentModel;
using System.Globalization;

namespace HandyControl.Tools
{
    [TypeConverter(typeof(LocalizationProviderTypeConverter))]
    public interface ILocalizationProvider
    {
        string Localize(string key, CultureInfo culture);
    }
}
