using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace HandyControl.Tools
{
    [MarkupExtensionReturnType(typeof(string))]
    public sealed class LocalizationExtension : LocalizationExtensionBase<string>
    {
        public LocalizationExtension()
        {
        }

        public LocalizationExtension(string key)
            : base(key)
        {
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            this.ThrowIfKeyNotSpecified();

            MultiBinding binding = new MultiBinding();

            binding.Bindings.Add(new Binding() { Source = this.Key });

            IProvideValueTarget provideValueTarget = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;

            if (provideValueTarget != null)
            {
                if (provideValueTarget.TargetObject is DependencyObject targetObject)
                {
                    var bindingSource = new LocalizationPropertyBindingSource(LocalizationManager.ProviderProperty, targetObject);

                    binding.Bindings.Add(bindingSource.Binding);

                    binding.Converter = new LocalizationConverter(targetObject, this.Provider, this.Language, this.StringFormat, this.FallbackValue);

                    return binding.ProvideValue(serviceProvider);
                }
                else
                {
                    return this;
                }
            }

            return base.ProvideValue(serviceProvider);
        }
    }
}
