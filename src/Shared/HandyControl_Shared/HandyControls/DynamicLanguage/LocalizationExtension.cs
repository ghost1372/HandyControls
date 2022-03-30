using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Xaml;
using HandyControl.Tools.Extension;

namespace HandyControl.Tools;

/// <summary>
/// A markup extension that returns a localized string by key or binding
/// </summary>
[ContentProperty(nameof(ArgumentBindings))]
public class LocalizationExtension : MarkupExtension
{
    private Collection<BindingBase> _arguments;

    public LocalizationExtension()
    {
    }

    public LocalizationExtension(string key)
    {
        Key = key;
    }

    /// <summary>
    /// Localized string key
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// Key binding of a localized string
    /// </summary>
    public Binding KeyBinding { get; set; }

    /// <summary>
    /// Formatted localized string arguments
    /// </summary>
    public IEnumerable<object> Arguments { get; set; }

    /// <summary>
    /// Formatted localized string argument bindings
    /// </summary>
    public Collection<BindingBase> ArgumentBindings
    {
        get { return _arguments ?? (_arguments = new Collection<BindingBase>()); }
        set { _arguments = value; }
    }
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (Key != null && KeyBinding != null)
            throw new ArgumentException($"Cannot be set at the same time {nameof(Key)} и {nameof(KeyBinding)}");
        if (Key == null && KeyBinding == null)
            throw new ArgumentException($"Must set {nameof(Key)} or {nameof(KeyBinding)}");
        if (Arguments != null && ArgumentBindings.Any())
            throw new ArgumentException($"Cannot be set at the same time {nameof(Arguments)} и {nameof(ArgumentBindings)}");

        var target = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));
        if (target.TargetObject.GetType().FullName == "System.Windows.SharedDp")
            return this;

        // we do this for multi resource support
        // first, we find our host (window/usercontrol)
        var hostRoot = serviceProvider.GetService(typeof(IRootObjectProvider)) as IRootObjectProvider;
        var host = hostRoot.RootObject;

        ILocalizationProvider ctlProvider = null;
        if (target.TargetObject is DependencyObject ctl)
        {
            // second, we get the control provider
            object localValue = ctl.ReadLocalValue(LocalizationManager.ProviderProperty);
            if (localValue != DependencyProperty.UnsetValue)
            {
                if (localValue is ILocalizationProvider provider)
                {
                    ctlProvider = provider;
                }
            }

            // if provider is null, we need to find control parents
            if (ctlProvider == null)
            {
                var parents = ctl.FindAscendants();
                foreach (var item in parents)
                {
                    localValue = item.ReadLocalValue(LocalizationManager.ProviderProperty);
                    if (localValue != DependencyProperty.UnsetValue)
                    {
                        if (localValue is ILocalizationProvider provider)
                        {
                            ctlProvider = provider;
                            if (provider != null)
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }

        // if provider is null again, we need to check the host provider
        if (ctlProvider == null)
        {
            if (host is DependencyObject mainHost)
            {
                object localValue = mainHost.ReadLocalValue(LocalizationManager.ProviderProperty);
                if (localValue != DependencyProperty.UnsetValue)
                {
                    if (localValue is ILocalizationProvider hostProvider)
                    {
                        ctlProvider = hostProvider;
                    }
                }
            }
        }

        // if provider is null again, mybe we are in usercontrol so we can use mainwindow provider
        if (ctlProvider == null)
        {
            if (Application.Current.MainWindow != null)
            {
                object localValue = Application.Current.MainWindow.ReadLocalValue(LocalizationManager.ProviderProperty);
                if (localValue != DependencyProperty.UnsetValue)
                {
                    if (localValue is ILocalizationProvider provider)
                    {
                        ctlProvider = provider;
                    }
                }
            }
        }

        // If a key binding or argument binding list is given,
        // then use BindingLocalizationListener
        if (KeyBinding != null || ArgumentBindings.Any())
        {
            var listener = new BindingLocalizationListener();

            // Create a binding for the listener
            var listenerBinding = new Binding { Source = listener };

            var keyBinding = KeyBinding ?? new Binding { Source = Key };

            var multiBinding = new MultiBinding
            {
                Converter = new BindingLocalizationConverter(ctlProvider),
                ConverterParameter = Arguments,
                Bindings = { listenerBinding, keyBinding }
            };

            // Add all passed argument bindings
            foreach (var binding in ArgumentBindings)
                multiBinding.Bindings.Add(binding);

            var value = multiBinding.ProvideValue(serviceProvider);
            // Save the binding expression in the listener
            listener.SetBinding(value as BindingExpressionBase);
            return value;
        }

        // If the key is specified, then use KeyLocalizationListener
        if (!string.IsNullOrEmpty(Key))
        {
            var listener = new KeyLocalizationListener(ctlProvider, Key, Arguments?.ToArray());

            // If localization is hung on the DependencyProperty of an object DependencyObject
            if ((target.TargetObject is DependencyObject && target.TargetProperty is DependencyProperty) ||
                target.TargetObject is Setter)
            {
                var binding = new Binding(nameof(KeyLocalizationListener.Value))
                {
                    Source = listener,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                };
                return binding.ProvideValue(serviceProvider);
            }

            // If localization is hung on Binding, then we return the listener
            var targetBinding = target.TargetObject as Binding;
            if (targetBinding != null && target.TargetProperty != null &&
                target.TargetProperty.GetType().FullName == "System.Reflection.RuntimePropertyInfo")
            {
                targetBinding.Path = new PropertyPath(nameof(KeyLocalizationListener.Value));
                targetBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                return listener;
            }

            // Otherwise, return the localized string
            return listener.Value;
        }

        return null;
    }
}
