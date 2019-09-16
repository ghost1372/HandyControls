// https://www.codeproject.com/Articles/459958/Bindable-Converter-Converter-Parameter-and-StringF

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace HandyControl.Controls
{
    public class BindableBinding : MarkupExtension
    {
        private readonly Binding binding = new Binding();

        public BindableBinding()
        {

        }

        public BindableBinding(PropertyPath path)
        {
            binding.Path = path;
        }

        public PropertyPath Path
        {
            get
            {
                return binding.Path;
            }
            set
            {
                binding.Path = value;
            }
        }

        public Object Source
        {
            get
            {
                return binding.Source;
            }
            set
            {
                binding.Source = value;
            }
        }

        public RelativeSource RelativeSource
        {
            get
            {
                return binding.RelativeSource;
            }
            set
            {
                binding.RelativeSource = value;
            }
        }

        public String ElementName
        {
            get
            {
                return binding.ElementName;
            }
            set
            {
                binding.ElementName = value;
            }
        }

        public String XPath
        {
            get
            {
                return binding.XPath;
            }
            set
            {
                binding.XPath = value;
            }
        }

        public BindingMode Mode
        {
            get
            {
                return binding.Mode;
            }
            set
            {
                binding.Mode = value;
            }
        }

        public CultureInfo ConverterCulture
        {
            get
            {
                return binding.ConverterCulture;
            }
            set
            {
                binding.ConverterCulture = value;
            }
        }

        public object ConverterParameter
        {
            get;
            set;
        }

        private Binding _converterParameterBinding;

        public Binding ConverterParameterBinding
        {
            get
            {
                return _converterParameterBinding;
            }
            set
            {
                _converterParameterBinding = value;
            }
        }

        public IValueConverter Converter
        {
            get;
            set;
        }

        private Binding _converterBinding;

        public Binding ConverterBinding
        {
            get
            {
                return _converterBinding;
            }
            set
            {
                _converterBinding = value;
            }
        }

        public String StringFormat
        {
            get;
            set;
        }

        private Binding _stringFormatBinding;

        public Binding StringFormatBinding
        {
            get
            {
                return _stringFormatBinding;
            }
            set
            {
                _stringFormatBinding = value;
            }
        }


        public override object ProvideValue(IServiceProvider serviceProvider)
        {

            MultiBinding multiBinding = new MultiBinding();
            multiBinding.Mode = binding.Mode;
            multiBinding.ConverterCulture = binding.ConverterCulture;
            multiBinding.Converter = new InternalConverter(this);
            multiBinding.Bindings.Add(binding);

            if (ConverterParameterBinding != null)
            {
                multiBinding.Bindings.Add(ConverterParameterBinding);
            }
            else
            {
                multiBinding.ConverterParameter = ConverterParameter;
            }

            if (ConverterBinding != null)
            {
                multiBinding.Bindings.Add(ConverterBinding);
            }

            if (StringFormatBinding != null)
            {
                multiBinding.Bindings.Add(StringFormatBinding);
            }

            return multiBinding.ProvideValue(serviceProvider);
        }

        private class InternalConverter : IMultiValueConverter
        {
            private readonly BindableBinding binding;
            private IValueConverter lastConverter;
            private object lastConverterParameter;

            public InternalConverter(BindableBinding binding)
            {
                this.binding = binding;
            }

            object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                int valueIndex = 1;

                object converterParameter = parameter;
                if (binding.ConverterParameterBinding != null)
                {
                    converterParameter = values[valueIndex++];
                }
                lastConverterParameter = converterParameter;

                IValueConverter converter = binding.Converter as IValueConverter;
                if (binding.ConverterBinding != null)
                {
                    converter = values[valueIndex++] as IValueConverter;
                }
                lastConverter = converter;

                String stringFormat = binding.StringFormat as String;
                if (binding.StringFormatBinding != null)
                {
                    stringFormat = values[valueIndex++] as String;
                }

                object value = values[0];
                if (converter != null)
                {
                    value = converter.Convert(value, targetType, converterParameter, culture);
                }
                if (stringFormat != null)
                {
                    value = String.Format(stringFormat, value);
                }
                return value;
            }

            object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
            {

                if (lastConverter != null)
                {
                    return new object[] { lastConverter.ConvertBack(value, targetTypes[0], lastConverterParameter, culture) };

                }
                else
                {
                    return new object[] { value };
                }
            }
        }
    }
}
