using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Resources;
using System.Windows;
using HandyControl.Data;
using HandyControl.Tools.Extension;

namespace HandyControl.Tools;

/// <summary>
/// The main class for working with localization
/// </summary>
public class LocalizationManager
{
    public static Dictionary<string, ResxLocalizationProvider> AvailableResxProvider = new Dictionary<string, ResxLocalizationProvider>();
    
    public static Dictionary<string, ResourceManager> AvailableResourceManager = new Dictionary<string, ResourceManager>();
    
    private static LocalizationManager _localizationManager;

    public static LocalizationManager Instance => _localizationManager ?? (_localizationManager = new LocalizationManager());

    public event EventHandler<FunctionEventArgs<CultureInfo>> CultureChanged;

    public ILocalizationProvider LocalizationProvider { get; set; }
    private LocalizationManager() { }

    [DesignOnly(true)]
    public static readonly DependencyProperty DesignCultureProperty =
        DependencyProperty.RegisterAttached("DesignCulture", typeof(CultureInfo), typeof(LocalizationManager), new PropertyMetadata(OnDesignCultureChanged));

    [DesignOnly(true)]
    private static void OnDesignCultureChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        if (!DesignerProperties.GetIsInDesignMode(obj))
            return;

        ConfigHelper.Instance.SetLang(((CultureInfo)args.NewValue).Name);
    }

    [DesignOnly(true)]
    public static CultureInfo GetDesignCulture(DependencyObject obj)
    {
        if (DesignerProperties.GetIsInDesignMode(obj))
            return (CultureInfo) obj.GetValue(DesignCultureProperty);

        return null;
    }

    [DesignOnly(true)]
    public static void SetDesignCulture(DependencyObject obj, CultureInfo value)
    {
        if (DesignerProperties.GetIsInDesignMode(obj))
            obj.SetValue(DesignCultureProperty, value);
    }

    public static readonly DependencyProperty ProviderProperty = DependencyProperty.RegisterAttached("Provider", typeof(ILocalizationProvider),
            typeof(LocalizationManager), new PropertyMetadata(OnProviderChanged));

    private static void OnProviderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue != null)
        {
            Instance.LocalizationProvider = (ResxLocalizationProvider) e.NewValue;
        }
    }

    public static void SetProvider(UIElement dp, ILocalizationProvider value)
    {
        dp.SetValue(ProviderProperty, value);
    }

    public static ILocalizationProvider GetProvider(UIElement dp)
    {
        return (ILocalizationProvider) dp.GetValue(ProviderProperty);
    }

    internal void OnCultureChanged(CultureInfo culture)
    {
        CultureChanged?.Invoke(this, new FunctionEventArgs<CultureInfo>(culture));
    }

    public static object Localize(ILocalizationProvider provider, string key, CultureInfo cultureInfo = null)
    {
        if (string.IsNullOrEmpty(key))
            return "[NULL]";
        var localizedValue = provider?.Localize(key, cultureInfo);
        return localizedValue ?? $"[{key}]";
    }

    public static object Localize(string key, CultureInfo cultureInfo = null)
    {
        if (string.IsNullOrEmpty(key))
            return "[NULL]";

        foreach (var item in AvailableResxProvider)
        {
            ILocalizationProvider provider = item.Value;
            var value = provider.Localize(key, cultureInfo);
            if (!string.IsNullOrEmpty(value?.ToString()))
            {
                return value;
            }
        }

        return $"[{key}]";
    }

    public static string LocalizeString(ILocalizationProvider provider, string key, CultureInfo cultureInfo = null)
    {
        return Localize(provider, key, cultureInfo).ToString();
    }

    public static string LocalizeString(string key, CultureInfo cultureInfo = null)
    {
        return Localize(key, cultureInfo).ToString();
    }

    public static Dictionary<string, IEnumerable<CultureInfo>> GetAvailableCultures()
    {
        List<CultureInfo> result = new List<CultureInfo>();
        Dictionary<string, IEnumerable<CultureInfo>> availableCultures = new Dictionary<string, IEnumerable<CultureInfo>>();

        foreach (var item in AvailableResourceManager)
        {
            result.Clear();
            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            foreach (CultureInfo culture in cultures)
            {
                try
                {
                    if (culture.Equals(CultureInfo.InvariantCulture)) continue; //do not use "==", won't work

                    ResourceSet rs = item.Value.GetResourceSet(culture, true, false);
                    if (rs != null)
                        result.Add(culture);
                }
                catch (CultureNotFoundException)
                {
                    //NOP
                }
            }
            availableCultures.AddIfNotExists(item.Key, result);
        }
        return availableCultures;
    }
}
