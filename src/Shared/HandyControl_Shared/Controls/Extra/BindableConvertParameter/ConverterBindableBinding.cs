// https://www.codeproject.com/Articles/459958/Bindable-Converter-Converter-Parameter-and-StringF

using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace HandyControl.Controls
{
    public class ConverterBindableBinding : MarkupExtension
    {
        public Binding Binding { get; set; }

        public IValueConverter Converter { get; set; }

        public Binding ConverterParameterBinding { get; set; }

        public Binding ConverterBinding { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            MultiBinding multiBinding = new MultiBinding();
            multiBinding.Bindings.Add(Binding);
            multiBinding.Bindings.Add(ConverterParameterBinding);
            if (ConverterBinding != null) multiBinding.Bindings.Add(ConverterBinding);
            MultiValueConverterAdapter adapter = new MultiValueConverterAdapter();
            adapter.Converter = Converter;
            multiBinding.Converter = adapter;
            return multiBinding.ProvideValue(serviceProvider);
        }
    }
}
