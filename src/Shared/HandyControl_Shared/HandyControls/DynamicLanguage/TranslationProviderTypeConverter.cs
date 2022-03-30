using HandyControl.Tools.Extension;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace HandyControl.Tools;

/// <summary>
/// Provides multiple conversions from <see cref="string"/> to various translation providers.
/// </summary>
[Browsable(false)]
[EditorBrowsable(EditorBrowsableState.Never)]
internal class TranslationProviderTypeConverter : TypeConverter
{
    /// <summary>
    /// Returns whether this converter can convert the object to the specified type, using the specified context.
    /// </summary>
    /// <param name="context">An <see cref="System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
    /// <param name="destinationType">A <see cref="System.Type"/> that represents the type you want to convert to.</param>
    /// <returns><c>true</c>, if this converter can perform the conversion, otherwise <c>false</c>.</returns>
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
        return destinationType == typeof(ILocalizationProvider);
    }

    /// <summary>
    /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
    /// </summary>
    /// <param name="context">An <see cref="System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
    /// <param name="sourceType">A <see cref="System.Type"/> that represents the type you want to convert from.</param>
    /// <returns><c>true</c>, if this converter can perform the conversion, otherwise <c>false</c>.</returns>
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        return sourceType == typeof(string);
    }

    /// <summary>
    /// Converts the given object to the type of this converter, using the specified context and culture information.
    /// </summary>
    /// <param name="context">An <see cref="System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
    /// <param name="culture">The <see cref="System.Globalization.CultureInfo"/> to use as the current culture.</param>
    /// <param name="value">The <see cref="object"/> to convert.</param>
    /// <returns>An <see cref="object"/> that represents the converted value.</returns>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is string provider)
        {
            if (!string.IsNullOrEmpty(provider) && provider.Contains(";"))
            {
                var assemblyName = provider.Substring(0, provider.IndexOf(";"));
                string path = provider.ToString().Replace(";", ".");
                var resxProvider = new ResxLocalizationProvider(path, Assembly.Load(assemblyName));
                LocalizationManager.AvailableResxProvider.AddIfNotExists(path, resxProvider);
                return resxProvider;
            }
            else
            {
                throw new FormatException("translation provider is not a valid path. The path needs to have the following format: 'Assembly;FullClassName'.");
            }
        }

        return base.ConvertFrom(context, culture, value);
    }
}
