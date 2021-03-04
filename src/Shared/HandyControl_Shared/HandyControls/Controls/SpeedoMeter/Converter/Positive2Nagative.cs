using System;
using System.Globalization;
using System.Windows.Data;

namespace HandyControl.Tools.Converter
{
    public class Positive2Nagative : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value * -1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
