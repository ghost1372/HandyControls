// https://github.com/MartinKuschnik/Goji

using System;
using System.ComponentModel;
using System.Windows.Markup;

namespace HandyControl.Tools
{
    public abstract class LocalizationExtensionBase<T> : MarkupExtension
    {
        public LocalizationExtensionBase()
        {
        }

        public LocalizationExtensionBase(T key)
        {
            this.Key = key;
        }

        [ConstructorArgument("key")]
        public T Key { get; set; }

        public XmlLanguage Language { get; set; }

        public string StringFormat { get; set; }

        public string FallbackValue { get; set; }

        public ILocalizationProvider Provider { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            IProvideValueTarget provideValueTarget = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;

            if (provideValueTarget != null && provideValueTarget.TargetObject.GetType().FullName == "System.Windows.SharedDp")
            {
                return this;
            }

            return null;
        }

        protected internal void ThrowIfKeyNotSpecified()
        {
            if (this.Key == null)
            {
                throw new Exception("No localization key found. Use the constructor or the Key property to define one.");
            }
        }
    }
}
