// https://github.com/MartinKuschnik/Goji

using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace HandyControl.Tools
{
    [MarkupExtensionReturnType(typeof(XmlLanguage))]
    public class ApplicationLanguageExtension : MarkupExtension
    {
        public ApplicationLanguageExtension()
        {
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            ApplicationLanguageBindingSource bindingSource = new ApplicationLanguageBindingSource(Application.Current);

            Binding binding = bindingSource.Binding;

            return binding.ProvideValue(serviceProvider);
        }
    }
}
