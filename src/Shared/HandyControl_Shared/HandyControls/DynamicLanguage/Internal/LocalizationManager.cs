using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows;

namespace HandyControl.Tools
{
    public static class LocalizationManager
    {
        internal static Dictionary<string, ResxLocalizationProvider> AvailableResxProvider = new Dictionary<string, ResxLocalizationProvider>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public static readonly DependencyProperty ProviderProperty
            = DependencyProperty.RegisterAttached(
                "Provider",
                typeof(ILocalizationProvider),
                typeof(LocalizationManager),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits, OnProviderPropertyChanged));

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal static readonly DependencyProperty LocalizationPropertyChangedEventsProperty
            = DependencyProperty.RegisterAttachedReadOnly(
                "LocalizationPropertyChangedEvents",
                typeof(WeakDependencyPropertyEventBus),
                typeof(LocalizationManager),
                new FrameworkPropertyMetadata(new WeakDependencyPropertyEventBus())).DependencyProperty;

        public static void SetProvider(UIElement dp, ILocalizationProvider value)
        {
            dp.SetValue(ProviderProperty, value);
        }

        public static ILocalizationProvider GetProvider(UIElement dp)
        {
            return (ILocalizationProvider)dp.GetValue(ProviderProperty);
        }

        private static void OnProviderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            WeakDependencyPropertyEventBus events = d.GetValue(LocalizationPropertyChangedEventsProperty) as WeakDependencyPropertyEventBus;

            events.NotifySubscribers(d, args);
        }

        internal static void AddResxProvider(string BaseName, ResxLocalizationProvider resxLocalizationProvider)
        {
            if (!AvailableResxProvider.ContainsKey(BaseName))
            {
                AvailableResxProvider.Add(BaseName, resxLocalizationProvider);
            }
        }

        public static void ChangeCulture(CultureInfo cultureInfo)
        {
            Application.Current.SetCurrentUICulture(cultureInfo);
        }

        public static string Localize(string key, CultureInfo cultureInfo = null, string fallBackValue = null, string stringFormat = null)
        {
            foreach (var item in AvailableResxProvider)
            {
                ILocalizationProvider provider = item.Value;
                var value = provider.Localize(key, cultureInfo, fallBackValue, stringFormat);
                if (!string.IsNullOrEmpty(value))
                {
                    return value;
                }
            }

            return $"[{key}]";
        }

    }
}
