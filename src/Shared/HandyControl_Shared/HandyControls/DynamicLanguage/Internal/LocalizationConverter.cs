// https://github.com/MartinKuschnik/Goji

using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace HandyControl.Tools
{
    internal sealed class LocalizationConverter : IValueConverter, IMultiValueConverter
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly string fallbackValue;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ILocalizationProvider localizationProvider;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly XmlLanguage xmlLanguage;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly DependencyObject targetObject;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly string stringFormat;

        internal LocalizationConverter(DependencyObject targetObject, ILocalizationProvider provider = null, XmlLanguage language = null, string stringFormat = null, string fallbackValue = null)
        {
            if (targetObject == null)
            {
                throw new ArgumentNullException(MethodBase.GetCurrentMethod().GetParameters()[0].Name);
            }

            this.targetObject = targetObject;
            this.localizationProvider = provider;
            this.xmlLanguage = language;
            this.stringFormat = stringFormat;
            this.fallbackValue = fallbackValue;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ILocalizationProvider provider = this.localizationProvider;
            CultureInfo usedCulture = culture;
            string fallbackValue = this.fallbackValue ?? value.ToString();

            if (provider == null)
            {
                provider = this.targetObject.GetValue(LocalizationManager.ProviderProperty) as ILocalizationProvider;

                if (provider == null)
                {
                    if (string.IsNullOrEmpty(this.stringFormat))
                    {
                        return fallbackValue;
                    }
                    else
                    {
                        return string.Format(this.stringFormat, fallbackValue);
                    }
                }
            }

            if (this.xmlLanguage != null)
            {
                usedCulture = new CultureInfo(this.xmlLanguage.ToString());
            }

            return provider.Localize(value.ToString(), usedCulture, fallbackValue, this.stringFormat);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (!values.Any() || (!(values[0] is string) && values[0].GetType().FullName == "MS.Internal.NamedObject"))
            {
                return null;
            }

            return this.Convert(values[0], targetType, parameter, culture);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
