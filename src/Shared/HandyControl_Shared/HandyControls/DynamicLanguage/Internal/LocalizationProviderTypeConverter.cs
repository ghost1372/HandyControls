using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace HandyControl.Tools
{
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal class LocalizationProviderTypeConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(ILocalizationProvider);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string provider)
            {
                if (!provider.Contains(";"))
                {
                    throw new FormatException("Invalid localization provider path.");
                }
                if (!string.IsNullOrEmpty(provider) && provider.Contains(";"))
                {
                    var assemblyName = provider.Substring(0, provider.IndexOf(";"));
                    string path = provider.ToString().Replace(";", ".");
                    var resxProvider = new ResxLocalizationProvider(path, Assembly.Load(assemblyName));
                    LocalizationManager.AddResxProvider(path, resxProvider);
                    return resxProvider;
                }
                else
                {
                    throw new FormatException("localization provider is not a valid path. The path needs to have the following format: 'Assembly;FullClassName'.");
                }
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}
