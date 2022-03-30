using System.ComponentModel;
using System.Globalization;

namespace HandyControl.Tools;

/// <summary>
/// Interface for implementing a localized string provider
/// </summary>
[TypeConverter(typeof(TranslationProviderTypeConverter))]
public interface ILocalizationProvider
{
    /// <summary>
    /// Returns a localized object by key
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cultureInfo"></param>
    /// <returns></returns>
    object Localize(string key, CultureInfo cultureInfo);
}
