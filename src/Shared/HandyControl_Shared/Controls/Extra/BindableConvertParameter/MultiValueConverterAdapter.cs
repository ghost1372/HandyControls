// https://www.codeproject.com/Articles/459958/Bindable-Converter-Converter-Parameter-and-StringF

using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace HandyControl.Controls
{
    [ContentProperty("Converter")]
    internal class MultiValueConverterAdapter : IMultiValueConverter
    {
        public IValueConverter Converter { get; set; }

        #region IMultiValueConverter Members

        private object lastParameter;
        private IValueConverter lastConverter;

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            lastConverter = Converter;
            if (values.Length > 1) lastParameter = values[1];
            if (values.Length > 2) lastConverter = (IValueConverter)values[2];
            if (Converter == null) return values[0];
            return Converter.Convert(values[0], targetType, lastParameter, culture);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            if (lastConverter == null) return new object[] { value };
            return new object[] { lastConverter.ConvertBack(value, targetTypes[0], lastParameter, culture) };
        }

        #endregion

    }
}
